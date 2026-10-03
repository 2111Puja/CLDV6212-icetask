using System.ComponentModel.DataAnnotations;
using CommunityPortal.Web.Models;

namespace Community_Portal.Models
{
    /// <summary>Form model for the public "Report a Problem" page.</summary>
    public class CreateServiceRequestViewModel
    {
        public static readonly string[] IssueTypes =
        {
            "Water Leak / Pipe Burst",
            "Pothole / Road Repair",
            "Broken Streetlight",
            "Uncollected Refuse / Illegal Dumping",
            "Sewer Blockage / Overflow",
            "Other"
        };

        [Required(ErrorMessage = "Please select an issue type.")]
        [Display(Name = "Issue type")]
        public string IssueType { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please describe the problem.")]
        [StringLength(2000, MinimumLength = 10, ErrorMessage = "Please give at least 10 characters (maximum 2000).")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please tell us where the problem is.")]
        [StringLength(250, ErrorMessage = "The location may not be longer than 250 characters.")]
        [Display(Name = "Location address")]
        public string Location { get; set; } = string.Empty;

        [Display(Name = "Photo (optional)")]
        public IFormFile? Photo { get; set; }
    }

    /// <summary>Result of a lookup on the public tracking page.</summary>
    public class TrackViewModel
    {
        public string? Query { get; set; }
        public ServiceRequest? Request { get; set; }
        public string? Error { get; set; }
        public bool Searched => !string.IsNullOrWhiteSpace(Query);
    }

    /// <summary>Data shown on the municipality staff dashboard.</summary>
    public class DashboardViewModel
    {
        public List<ServiceRequest> Requests { get; set; } = new();
        public int Total { get; set; }
        public int New { get; set; }
        public int InProgress { get; set; }
        public int Resolved { get; set; }
        public int Urgent { get; set; }
        public string Filter { get; set; } = "All";
        public string? Search { get; set; }
        public string? LoadError { get; set; }
    }

    /// <summary>Maps statuses/priorities to the Bootstrap/theme badge classes used by the views.</summary>
    public static class BadgeHelper
    {
        public static string Status(RequestStatus status) => status switch
        {
            RequestStatus.InProgress => "badge-status-progress",
            RequestStatus.Resolved => "badge-status-resolved",
            RequestStatus.Closed => "badge-status-closed",
            _ => "badge-status-submitted"
        };

        public static string Priority(PriorityLevel priority) => priority switch
        {
            PriorityLevel.Urgent => "badge-priority-urgent",
            PriorityLevel.High => "badge-priority-high",
            PriorityLevel.Medium => "badge-priority-medium",
            _ => "badge-priority-low"
        };
    }
}
