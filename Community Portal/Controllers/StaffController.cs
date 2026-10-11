/* Microsoft. 2026. ASP.NET Core MVC Controllers and Action Results Overview. [Online].
 * Available at: <https://learn.microsoft.com/en-us/aspnet/core/mvc/controllers/actions>
 * [Accessed 2 October 2026].
 *
 * Microsoft. 2026. Model Binding in ASP.NET Core. [Online].
 * Available at: <https://learn.microsoft.com/en-us/aspnet/core/mvc/models/model-binding>
 * [Accessed 2 October 2026].*/

using Community_Portal.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CommunityPortal.Web.Models;

namespace Community_Portal.Controllers
{
    public class StaffController : Controller
    {
        private readonly ApplicationDbContext _context;

        // The dashboard reads the same Supabase table that residents submit their reports into,
        // so every report entered on "Report a Problem" appears here.
        public StaffController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Staff/Dashboard
        [HttpGet]
        public async Task<IActionResult> Dashboard(string? statusFilter)
        {
            var requests = _context.ServiceRequests.AsNoTracking().AsQueryable();

            if (!string.IsNullOrEmpty(statusFilter) && !statusFilter.Equals("All", StringComparison.OrdinalIgnoreCase))
            {
                if (Enum.TryParse<RequestStatus>(statusFilter.Replace(" ", ""), true, out var parsedStatus))
                {
                    requests = requests.Where(r => r.Status == parsedStatus);
                }
            }

            // Summary counters always reflect the whole table, not just the filtered rows
            ViewBag.CurrentFilter = statusFilter ?? "All";
            ViewBag.TotalCount = await _context.ServiceRequests.CountAsync();
            ViewBag.PendingCount = await _context.ServiceRequests.CountAsync(r => r.Status == RequestStatus.Pending);
            ViewBag.InProgressCount = await _context.ServiceRequests.CountAsync(r => r.Status == RequestStatus.InProgress);
            ViewBag.ResolvedCount = await _context.ServiceRequests.CountAsync(r => r.Status == RequestStatus.Resolved);

            // Newest reports first
            var list = await requests.OrderByDescending(r => r.SubmittedOn).ToListAsync();

            return View(list);
        }

        // GET: /Staff/Details/RPT-2026-000123
        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var lookup = id.Trim().ToUpperInvariant();
            var request = await _context.ServiceRequests
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.ReferenceNumber == lookup);

            if (request == null)
            {
                return NotFound();
            }

            return View(request);
        }

        // POST: /Staff/UpdateStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(ServiceRequest model, string referenceNumber)
        {
            // Fallback for key identification
            string targetRef = !string.IsNullOrEmpty(model.ReferenceNumber) ? model.ReferenceNumber : referenceNumber;

            if (string.IsNullOrEmpty(targetRef))
            {
                return NotFound();
            }

            var lookup = targetRef.Trim().ToUpperInvariant();
            var request = await _context.ServiceRequests.FirstOrDefaultAsync(r => r.ReferenceNumber == lookup);

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

                // Save the change to the database so residents see it on the Track page
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Request {targetRef} updated successfully.";
            }

            return RedirectToAction("Dashboard");
        }
    }
}
