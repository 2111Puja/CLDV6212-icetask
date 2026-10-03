/* Microsoft. 2026. Entity Framework Core - Executing raw SQL queries. [Online].
 * Available at: <https://learn.microsoft.com/en-us/ef/core/querying/sql-queries>
 * [Accessed 3 October 2026].*/

using Community_Portal.Data;
using Microsoft.EntityFrameworkCore;

namespace Community_Portal.Services
{
    /// <summary>
    /// Creates / upgrades the Supabase tables at start-up.
    ///
    /// Why not EnsureCreated()? EnsureCreated does NOTHING when the database already contains any table
    /// (Supabase projects ship with tables) and it can never add a column to a table that already exists,
    /// so after the model changed the live database stayed out of sync and every insert returned HTTP 500.
    /// The statements below are idempotent: safe to run on every start, on an empty database, and on a
    /// database created by an older version of this app.
    /// </summary>
    public static class DatabaseInitializer
    {
        private const string SchemaSql = @"
CREATE TABLE IF NOT EXISTS ""ServiceRequests"" (
    ""ReferenceNumber"" text NOT NULL,
    CONSTRAINT ""PK_ServiceRequests"" PRIMARY KEY (""ReferenceNumber"")
);
ALTER TABLE ""ServiceRequests"" ADD COLUMN IF NOT EXISTS ""IssueType"" text NOT NULL DEFAULT '';
ALTER TABLE ""ServiceRequests"" ADD COLUMN IF NOT EXISTS ""Description"" text NOT NULL DEFAULT '';
ALTER TABLE ""ServiceRequests"" ADD COLUMN IF NOT EXISTS ""Location"" text NOT NULL DEFAULT '';
ALTER TABLE ""ServiceRequests"" ADD COLUMN IF NOT EXISTS ""Status"" integer NOT NULL DEFAULT 1;
ALTER TABLE ""ServiceRequests"" ADD COLUMN IF NOT EXISTS ""Priority"" integer NOT NULL DEFAULT 1;
ALTER TABLE ""ServiceRequests"" ADD COLUMN IF NOT EXISTS ""SubmittedOn"" timestamp with time zone NOT NULL DEFAULT now();
ALTER TABLE ""ServiceRequests"" ADD COLUMN IF NOT EXISTS ""UpdatedOn"" timestamp with time zone NULL;
ALTER TABLE ""ServiceRequests"" ADD COLUMN IF NOT EXISTS ""AssignedTeam"" text NOT NULL DEFAULT 'Unassigned';
ALTER TABLE ""ServiceRequests"" ADD COLUMN IF NOT EXISTS ""StaffNotes"" text NULL;
ALTER TABLE ""ServiceRequests"" ADD COLUMN IF NOT EXISTS ""HasPhoto"" boolean NOT NULL DEFAULT false;
ALTER TABLE ""ServiceRequests"" ADD COLUMN IF NOT EXISTS ""PhotoData"" bytea NULL;
ALTER TABLE ""ServiceRequests"" ADD COLUMN IF NOT EXISTS ""PhotoContentType"" text NULL;
CREATE INDEX IF NOT EXISTS ""IX_ServiceRequests_SubmittedOn"" ON ""ServiceRequests"" (""SubmittedOn"");
CREATE INDEX IF NOT EXISTS ""IX_ServiceRequests_Status"" ON ""ServiceRequests"" (""Status"");
";

        // Supabase exposes every table in the public schema through its REST API. The app talks to Postgres
        // directly (the table owner bypasses RLS), so switching RLS on simply closes the public REST door.
        private const string RlsSql = @"ALTER TABLE ""ServiceRequests"" ENABLE ROW LEVEL SECURITY;";

        public static async Task InitializeAsync(IServiceProvider services, ILogger logger)
        {
            const int maxAttempts = 4;

            for (var attempt = 1; attempt <= maxAttempts; attempt++)
            {
                try
                {
                    using var scope = services.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    await db.Database.ExecuteSqlRawAsync(SchemaSql);
                    logger.LogInformation("Database schema verified (attempt {Attempt}).", attempt);

                    try
                    {
                        await db.Database.ExecuteSqlRawAsync(RlsSql);
                    }
                    catch (Exception ex)
                    {
                        logger.LogWarning(ex, "Could not enable row level security (non-fatal).");
                    }

                    return;
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Database initialisation failed (attempt {Attempt} of {Max}).", attempt, maxAttempts);

                    if (attempt < maxAttempts)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(3 * attempt));
                    }
                }
            }

            // Deliberately do not crash: the site still serves pages and /health reports the real problem.
            logger.LogCritical("The database could not be initialised. Check the connection string and visit /health.");
        }
    }
}
