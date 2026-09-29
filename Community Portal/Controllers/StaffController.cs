using Microsoft.AspNetCore.Mvc;
using CommunityPortal.Web.Models;

namespace Community_Portal.Controllers
{
    public class StaffController : Controller
    {
        // Mock dataset mapped directly to CommunityPortal.Web.Models.ServiceRequest
        private static readonly List<ServiceRequest> MockRequests = new()
        {
            new ServiceRequest
            {
                ReferenceNumber = "RPT-2026-000123",
                IssueType = "Water Leak / Pipe Burst",
                Location = "123 Main Road, Central",
                Status = RequestStatus.InProgress,
                Priority = PriorityLevel.High,
                SubmittedOn = DateTime.Parse("2026-05-18"),
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
                SubmittedOn = DateTime.Parse("2026-05-19"),
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
                SubmittedOn = DateTime.Parse("2026-05-15"),
                Description = "Streetlight flickering and completely dark at night.",
                AssignedTeam = "Electrical Services Team 1"
            }
        };

        // GET: /Staff/Dashboard
        [HttpGet]
        public IActionResult Dashboard(string statusFilter)
        {
            var requests = MockRequests.AsEnumerable();

            if (!string.IsNullOrEmpty(statusFilter) && !statusFilter.Equals("All", StringComparison.OrdinalIgnoreCase))
            {
                if (Enum.TryParse<RequestStatus>(statusFilter.Replace(" ", ""), true, out var parsedStatus))
                {
                    requests = requests.Where(r => r.Status == parsedStatus);
                }
            }

            ViewBag.CurrentFilter = statusFilter ?? "All";
            ViewBag.TotalCount = MockRequests.Count;
            ViewBag.PendingCount = MockRequests.Count(r => r.Status == RequestStatus.Pending);
            ViewBag.InProgressCount = MockRequests.Count(r => r.Status == RequestStatus.InProgress);
            ViewBag.ResolvedCount = MockRequests.Count(r => r.Status == RequestStatus.Resolved);

            return View(requests.ToList());
        }

        // GET: /Staff/Details/RPT-2026-000123
        [HttpGet]
        public IActionResult Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var request = MockRequests.FirstOrDefault(r => r.ReferenceNumber.Equals(id, StringComparison.OrdinalIgnoreCase));

            if (request == null)
            {
                return NotFound();
            }

            return View(request);
        }

        // POST: /Staff/UpdateStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateStatus(ServiceRequest model, string referenceNumber)
        {
            // Fallback for key identification
            string targetRef = !string.IsNullOrEmpty(model.ReferenceNumber) ? model.ReferenceNumber : referenceNumber;

            var request = MockRequests.FirstOrDefault(r => r.ReferenceNumber.Equals(targetRef, StringComparison.OrdinalIgnoreCase));

            if (request != null)
            {
                // Update Status, Priority, Staff Notes, and Assigned Team
                request.Status = model.Status;
                request.Priority = model.Priority;

                if (!string.IsNullOrEmpty(model.StaffNotes))
                {
                    request.StaffNotes = model.StaffNotes;
                }

                if (!string.IsNullOrEmpty(model.AssignedTeam))
                {
                    request.AssignedTeam = model.AssignedTeam;
                }

                TempData["SuccessMessage"] = $"Request {targetRef} updated successfully.";
            }

            return RedirectToAction("Dashboard");
        }
    }
}