/* Microsoft. 2026. Entity Framework Core DbContext Configuration and Initialization. [Online].
 * Available at: <https://learn.microsoft.com/en-us/ef/core/>
 * [Accessed 2 October 2026].
 *
 * Microsoft. 2026. Cookie authentication in ASP.NET Core. [Online].
 * Available at: <https://learn.microsoft.com/en-us/aspnet/core/security/authentication/cookie>
 * [Accessed 3 October 2026].
 *
 * Microsoft. 2026. Configure ASP.NET Core to work with proxy servers and load balancers. [Online].
 * Available at: <https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/proxy-load-balancer>
 * [Accessed 3 October 2026].*/

using Community_Portal.Data;
using Community_Portal.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Community_Portal
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Render tells the container which port to listen on through the PORT variable.
            var renderPort = Environment.GetEnvironmentVariable("PORT");
            if (!string.IsNullOrWhiteSpace(renderPort) &&
                string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("ASPNETCORE_URLS")))
            {
                builder.WebHost.UseUrls($"http://0.0.0.0:{renderPort}");
            }

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // Register Supabase (PostgreSQL) Database Context
            var database = ConnectionStringResolver.Resolve(builder.Configuration);
            builder.Services.AddSingleton(database);
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(database.ConnectionString, npgsql => npgsql.EnableRetryOnFailure(3)));

            // Municipality staff authentication (cookie based)
            builder.Services.Configure<StaffAccountOptions>(builder.Configuration.GetSection("Staff"));
            builder.Services
                .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Account/Login";
                    options.AccessDeniedPath = "/Account/Login";
                    options.Cookie.Name = "CommunityPortal.Staff";
                    options.Cookie.HttpOnly = true;
                    options.Cookie.SameSite = SameSiteMode.Lax;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                    options.ExpireTimeSpan = TimeSpan.FromHours(8);
                    options.SlidingExpiration = true;
                });
            builder.Services.AddAuthorization();

            // Render terminates HTTPS in front of the container; trust its forwarded headers.
            builder.Services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
                options.KnownNetworks.Clear();
                options.KnownProxies.Clear();
            });

            // 1. Add Swagger services
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            if (database.Warning is not null)
            {
                app.Logger.LogWarning("Database configuration: {Warning}", database.Warning);
            }
            app.Logger.LogInformation("Database target: {Host}:{Port}", database.Host, database.Port);

            app.UseForwardedHeaders();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            else
            {
                app.UseHttpsRedirection();
            }

            // Friendly 404 / 403 / etc. pages instead of a blank browser error.
            app.UseStatusCodePagesWithReExecute("/Home/HttpStatus", "?code={0}");

            // 2. Enable Swagger middleware explicitly with production endpoints
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Community Portal API v1");
                c.RoutePrefix = "swagger"; // Keeps it accessible at /swagger
            });

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            // Health check: open /health on the live site to see exactly why the database is (not) working.
            app.MapGet("/health", async (ApplicationDbContext db, ConnectionStringResolver.Result target, ILogger<Program> logger) =>
            {
                try
                {
                    var count = await db.ServiceRequests.CountAsync();
                    return Results.Json(new { status = "healthy", database = "connected", requests = count });
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Health check failed.");

                    var problem = ex.GetBaseException() switch
                    {
                        PostgresException { SqlState: "28P01" } => "Authentication failed - check the database user and password.",
                        PostgresException { SqlState: "42P01" } => "The ServiceRequests table does not exist yet - restart the service so it is created.",
                        PostgresException pg => $"PostgreSQL error {pg.SqlState}.",
                        System.Net.Sockets.SocketException => "Cannot reach the database host. On Render use the Supabase Session pooler host, not the direct db.<ref>.supabase.co host.",
                        _ => "Unexpected database error - see the Render logs."
                    };

                    return Results.Json(
                        new { status = "unhealthy", configured = target.IsConfigured, host = target.Host, problem, hint = target.Warning },
                        statusCode: StatusCodes.Status503ServiceUnavailable);
                }
            }).ExcludeFromDescription();

            // Create / upgrade the database schema (replaces EnsureCreated, which silently skips existing databases).
            // Failures are logged with full detail but do not crash the site.
            await DatabaseInitializer.InitializeAsync(app.Services, app.Logger);

            await app.RunAsync();
        }
    }
}
