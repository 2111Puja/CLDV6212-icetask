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


        public DbSet<ServiceRequest> ServiceRequests => Set<ServiceRequest>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ServiceRequest>(entity =>
            {
                entity.HasKey(r => r.ReferenceNumber);

                entity.Property(r => r.ReferenceNumber).HasMaxLength(30);
                entity.Property(r => r.IssueType).IsRequired().HasMaxLength(100);
                entity.Property(r => r.Location).IsRequired().HasMaxLength(200);
                entity.Property(r => r.Description).IsRequired();
                entity.Property(r => r.AssignedTeam).HasMaxLength(100);
                // Store enums as readable text instead of numbers
                entity.Property(r => r.Status).HasConversion<string>().HasMaxLength(20);
                entity.Property(r => r.Priority).HasConversion<string>().HasMaxLength(20);

                // Starter data (same three requests the dashboard used to hard-code)
                entity.HasData(
                    new ServiceRequest
                    {
                        ReferenceNumber = "RPT-2026-000123",
                        IssueType = "Water Leak / Pipe Burst",
                        Location = "123 Main Road, Central",
                        Status = RequestStatus.InProgress,
                        Priority = PriorityLevel.High,
                        SubmittedOn = new DateTime(2026, 5, 18, 0, 0, 0, DateTimeKind.Utc),
                        Description = "Major water leak near the public library flooding the pavement.",
                        AssignedTeam = "Water & Sanitation Unit B",
                        StaffNotes = "Technicians dispatched to isolate the main valve."
                    },
                    new ServiceRequest
                    {
                        ReferenceNumber = "RPT-2026-000124",
                        IssueType = "Pothole / Road Repair",
                        Location = "45 Elm Street, North Ward",
                        Status = RequestStatus.Pending,
                        Priority = PriorityLevel.Medium,
                        SubmittedOn = new DateTime(2026, 5, 19, 0, 0, 0, DateTimeKind.Utc),
                        Description = "Large pothole in the left lane causing traffic hazard.",
                        AssignedTeam = "Unassigned"
                    },
                    new ServiceRequest
                    {
                        ReferenceNumber = "RPT-2026-000125",
                        IssueType = "Broken Streetlight",
                        Location = "Corner 5th Ave & Pine St",
                        Status = RequestStatus.Resolved,
                        Priority = PriorityLevel.Low,
                        SubmittedOn = new DateTime(2026, 5, 15, 0, 0, 0, DateTimeKind.Utc),
                        Description = "Streetlight flickering and completely dark at night.",
                        AssignedTeam = "Electrical Services Team 1"
                    });
            });
        }
    }
}

