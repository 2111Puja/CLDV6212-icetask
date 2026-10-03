/* Microsoft. 2026. Entity Framework Core Data Annotations. [Online].
 * Available at: <https://learn.microsoft.com/en-us/ef/core/modeling/entity-properties>
 * [Accessed 3 October 2026].*/

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CommunityPortal.Web.Models
{
    public enum PriorityLevel
    {
        Low,
        Medium,
        High,
        Urgent
    }

    // NOTE: the numeric order of this enum must not change - the values are stored as integers in PostgreSQL.
    public enum RequestStatus
    {
        Pending,
        Submitted,

        [Display(Name = "In Progress")]
        InProgress,

        Resolved,
        Closed
    }

    public class ServiceRequest
    {
        [Key]
        public string ReferenceNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select an issue type.")]
        [StringLength(100)]
        [Display(Name = "Issue Type")]
        public string IssueType { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter a detailed description.")]
        [StringLength(2000)]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please specify the location.")]
        [StringLength(250)]
        public string Location { get; set; } = string.Empty;

        public RequestStatus Status { get; set; } = RequestStatus.Submitted;

        public PriorityLevel Priority { get; set; } = PriorityLevel.Medium;

        public DateTime SubmittedOn { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedOn { get; set; }

        [Display(Name = "Assigned Team")]
        public string AssignedTeam { get; set; } = "Unassigned";

        [Display(Name = "Staff Notes")]
        public string? StaffNotes { get; set; }

        // The photo bytes are stored in the database. Lists never load them (see QueryExtensions.WithoutPhoto);
        // HasPhoto tells the views whether one exists.
        public bool HasPhoto { get; set; }

        [JsonIgnore]
        public byte[]? PhotoData { get; set; }

        [JsonIgnore]
        public string? PhotoContentType { get; set; }

        // ---- Read-only helpers (not mapped to columns because they have no setter) ----

        // South African Standard Time is UTC+2 all year round (no daylight saving).
        public DateTime SubmittedOnLocal => SubmittedOn.ToUniversalTime().AddHours(2);

        public DateTime? UpdatedOnLocal => UpdatedOn?.ToUniversalTime().AddHours(2);

        [Display(Name = "Date Submitted")]
        public string DateSubmitted => SubmittedOnLocal.ToString("dd MMM yyyy");

        // Helper property to render "In Progress" with a space in Razor views
        public string StatusDisplayName => Status switch
        {
            RequestStatus.InProgress => "In Progress",
            RequestStatus.Pending => "Received",
            RequestStatus.Submitted => "Received",
            _ => Status.ToString()
        };

        // 0 = received, 1 = in progress, 2 = resolved (used by the tracking stepper)
        public int StatusStep => Status switch
        {
            RequestStatus.InProgress => 1,
            RequestStatus.Resolved => 2,
            RequestStatus.Closed => 2,
            _ => 0
        };
    }
}
