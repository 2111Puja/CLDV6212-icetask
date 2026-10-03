/* Npgsql. 2026. Connection String Parameters. [Online].
 * Available at: <https://www.npgsql.org/doc/connection-string-parameters.html>
 * [Accessed 3 October 2026].*/

using Npgsql;

namespace Community_Portal.Services
{
    /// <summary>
    /// Finds the PostgreSQL (Supabase) connection string no matter how it was supplied on Render, and
    /// normalises it so it works from a cloud container:
    ///  - accepts ConnectionStrings__DefaultConnection, ConnectionStrings__Default, DATABASE_URL or SUPABASE_CONNECTION_STRING
    ///  - accepts both "Host=...;Username=..." and "postgresql://user:pass@host:port/db" formats
    ///  - forces SSL (Supabase requires it) unless the host is local
    ///  - keeps the pool small so the Supabase pooler is not exhausted
    /// </summary>
    public static class ConnectionStringResolver
    {
        public sealed record Result(string ConnectionString, bool IsConfigured, string Host, int Port, string? Warning);

        public static Result Resolve(IConfiguration config)
        {
            var raw = config.GetConnectionString("DefaultConnection")
                      ?? config.GetConnectionString("Default")
                      ?? config["DATABASE_URL"]
                      ?? config["SUPABASE_CONNECTION_STRING"];

            if (string.IsNullOrWhiteSpace(raw))
            {
                // Placeholder so the app can still start and /health can explain what is wrong.
                return new Result("Host=connection-string-not-configured;Database=postgres;Username=none;Password=none",
                    false, "(not configured)", 0,
                    "No connection string found. Set ConnectionStrings__DefaultConnection on Render.");
            }

            raw = raw.Trim().Trim('"', '\'');

            var asKeywords = raw.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase) ||
                             raw.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase)
                ? FromUri(raw)
                : raw;

            var builder = new NpgsqlConnectionStringBuilder(asKeywords);

            var isLocal = builder.Host is null or "localhost" or "127.0.0.1" or "::1" or "db" or "postgres";
            if (!isLocal)
            {
                builder.SslMode = SslMode.Require;
                try
                {
                    // Supabase's certificate chain is not in the container trust store.
                    builder["Trust Server Certificate"] = true;
                }
                catch (ArgumentException)
                {
                    // Keyword not supported by this Npgsql version - SSL Mode=Require is still applied.
                }
            }

            if (builder.MaxPoolSize > 10)
            {
                builder.MaxPoolSize = 10;
            }

            // Supabase "transaction" pooler (port 6543) is PgBouncer in transaction mode.
            if (builder.Port == 6543)
            {
                builder.NoResetOnClose = true;
            }

            string? warning = null;
            var host = builder.Host ?? string.Empty;
            if (host.StartsWith("db.", StringComparison.OrdinalIgnoreCase) &&
                host.EndsWith(".supabase.co", StringComparison.OrdinalIgnoreCase))
            {
                warning = "This is the Supabase DIRECT host, which is IPv6-only. Render cannot reach IPv6, so " +
                          "connections fail with 'Network is unreachable'. Use the Session pooler host " +
                          "(aws-0-<region>.pooler.supabase.com, user postgres.<project-ref>) instead.";
            }

            return new Result(builder.ConnectionString, true, host, builder.Port, warning);
        }

        private static string FromUri(string uriText)
        {
            var uri = new Uri(uriText);
            var userInfo = uri.UserInfo.Split(':', 2);
            var database = uri.AbsolutePath.Trim('/');

            var builder = new NpgsqlConnectionStringBuilder
            {
                Host = uri.Host,
                Port = uri.Port > 0 ? uri.Port : 5432,
                Username = Uri.UnescapeDataString(userInfo[0]),
                Password = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : null,
                Database = string.IsNullOrWhiteSpace(database) ? "postgres" : database
            };
            return builder.ConnectionString;
        }
    }
}
