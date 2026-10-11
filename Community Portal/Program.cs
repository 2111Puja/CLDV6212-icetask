using Community_Portal.Data;
using Microsoft.EntityFrameworkCore;

namespace Community_Portal
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // Register Swagger generator services
            builder.Services.AddSwaggerGen();

            // Retrieve connection string from Render environment variables or configuration
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                                   ?? Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
                                   ?? Environment.GetEnvironmentVariable("DATABASE_URL");

            // Automatically parse Supabase URL format into Npgsql key=value format if necessary
            if (!string.IsNullOrEmpty(connectionString) && connectionString.StartsWith("postgres"))
            {
                try
                {
                    var uri = new Uri(connectionString);
                    var userInfo = uri.UserInfo.Split(':');
                    connectionString = $"Host={uri.Host};Port={uri.Port};Database={uri.AbsolutePath.TrimStart('/')};Username={userInfo[0]};Password={userInfo[1]};SslMode=Prefer;Trust Server Certificate=true";
                }
                catch
                {
                    // Fallback if parsing fails
                }
            }

            // Register ApplicationDbContext with PostgreSQL for Supabase
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(connectionString));

            var app = builder.Build();

            // Create the database tables (and starter data) if they don't exist yet.
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                db.Database.EnsureCreated();
            }

            app.UseForwardedHeaders();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            // Enable Swagger middleware explicitly with production endpoints
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Community Portal API v1");
                c.RoutePrefix = "swagger"; // Keeps it accessible at /swagger
            });

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthorization();

            app.MapStaticAssets();

            // Simple health check for Render and the CI pipeline
            app.MapGet("/health", () => Results.Ok("Healthy"));

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}