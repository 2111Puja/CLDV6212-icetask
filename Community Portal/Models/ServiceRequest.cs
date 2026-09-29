using System;
using System.ComponentModel.DataAnnotations;

namespace CommunityPortal.Web.Models
{
    public enum PriorityLevel
    {
        Low,
        Medium,
        High,
        Urgent
    }

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
        public string ReferenceNumber { get; set; } = "RPT-2026-" + new Random().Next(100000, 999999);

        [Required(ErrorMessage = "Please select an issue type.")]
        [Display(Name = "Issue Type")]
        public string IssueType { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter a detailed description.")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please specify the location.")]
        public string Location { get; set; } = string.Empty;

        public RequestStatus Status { get; set; } = RequestStatus.Pending;

        public PriorityLevel Priority { get; set; } = PriorityLevel.Medium;

        public DateTime SubmittedOn { get; set; } = DateTime.UtcNow;

        [Display(Name = "Date Submitted")]
        public string DateSubmitted => SubmittedOn.ToString("dd MMM yyyy");

        [Display(Name = "Assigned Team")]
        public string AssignedTeam { get; set; } = "Unassigned";

        public string? StaffNotes { get; set; }

        // Helper property to render "In Progress" with a space in Razor views
        public string StatusDisplayName => Status switch
        {
            RequestStatus.InProgress => "In Progress",
            _ => Status.ToString()
        };
    }
}