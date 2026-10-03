using Community_Portal.Models;
using CommunityPortal.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace Community_Portal.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // This represents your service requests table in Supabase
        public DbSet<ServiceRequest> ServiceRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ServiceRequest>(entity =>
            {
                entity.ToTable("ServiceRequests");
                entity.HasKey(r => r.ReferenceNumber);
                entity.HasIndex(r => r.SubmittedOn);
                entity.HasIndex(r => r.Status);

                // Computed, read-only helpers - never columns.
                entity.Ignore(r => r.SubmittedOnLocal);
                entity.Ignore(r => r.UpdatedOnLocal);
                entity.Ignore(r => r.DateSubmitted);
                entity.Ignore(r => r.StatusDisplayName);
                entity.Ignore(r => r.StatusStep);
            });
        }
    }

    public static class QueryExtensions
    {
        /// <summary>
        /// Projects a request WITHOUT its photo bytes so list/detail pages do not pull
        /// megabytes of image data out of the database on every request.
        /// </summary>
        public static IQueryable<ServiceRequest> WithoutPhoto(this IQueryable<ServiceRequest> query) =>
            query.Select(r => new ServiceRequest
            {
                ReferenceNumber = r.ReferenceNumber,
                IssueType = r.IssueType,
                Description = r.Description,
                Location = r.Location,
                Status = r.Status,
                Priority = r.Priority,
                SubmittedOn = r.SubmittedOn,
                UpdatedOn = r.UpdatedOn,
                AssignedTeam = r.AssignedTeam,
                StaffNotes = r.StaffNotes,
                HasPhoto = r.HasPhoto
            });
    }
}
