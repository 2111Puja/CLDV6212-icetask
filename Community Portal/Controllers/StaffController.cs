/* Microsoft. 2026. ASP.NET Core MVC Controllers and Action Results Overview. [Online].
 * Available at: <https://learn.microsoft.com/en-us/aspnet/core/mvc/controllers/actions>
 * [Accessed 2 October 2026].
 *
 * Microsoft. 2026. Model Binding in ASP.NET Core. [Online].
 * Available at: <https://learn.microsoft.com/en-us/aspnet/core/mvc/models/model-binding>
 * [Accessed 2 October 2026].
 *
 * Microsoft. 2026. Simple authorization in ASP.NET Core. [Online].
 * Available at: <https://learn.microsoft.com/en-us/aspnet/core/security/authorization/simple>
 * [Accessed 3 October 2026].*/

using Community_Portal.Data;
using Community_Portal.Models;
using CommunityPortal.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Community_Portal.Controllers
{
    [Authorize]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class StaffController : Controller
    {
        public static readonly string[] Teams =
        {
            "Unassigned",
            "Water & Sanitation Unit",
            "Roads & Stormwater Team",
            "Electrical Services Team",
            "Waste Management Unit"
        };

        private readonly ApplicationDbContext _context;
        private readonly ILogger<StaffController> _logger;

        public StaffController(ApplicationDbContext context, ILogger<StaffController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: /Staff/Dashboard
        [HttpGet]
        public async Task<IActionResult> Dashboard(string? statusFilter, string? search, CancellationToken cancellationToken)
        {
            var filter = string.IsNullOrWhiteSpace(statusFilter) ? "All" : statusFilter.Trim();
            var term = (search ?? string.Empty).Replace("%", "").Replace("_", "").Trim();

            var viewModel = new DashboardViewModel { Filter = filter, Search = search?.Trim() };

            try
            {
                var all = _context.ServiceRequests.AsNoTracking();

                // KPI counters (always for the whole table, independent of the filter)
                var counts = await all
                    .GroupBy(r => r.Status)
                    .Select(g => new { Status = g.Key, Count = g.Count() })
                    .ToListAsync(cancellationToken);

                int Count(params RequestStatus[] statuses) =>
                    counts.Where(c => statuses.Contains(c.Status)).Sum(c => c.Count);

                viewModel.Total = counts.Sum(c => c.Count);
                viewModel.New = Count(RequestStatus.Pending, RequestStatus.Submitted);
                viewModel.InProgress = Count(RequestStatus.InProgress);
                viewModel.Resolved = Count(RequestStatus.Resolved, RequestStatus.Closed);
                viewModel.Urgent = await all.CountAsync(r => r.Priority == PriorityLevel.Urgent &&
                                                             r.Status != RequestStatus.Resolved &&
                                                             r.Status != RequestStatus.Closed, cancellationToken);

                var query = all;

                switch (filter.ToLowerInvariant())
                {
                    case "new":
                    case "pending":
                    case "submitted":
                        query = query.Where(r => r.Status == RequestStatus.Pending || r.Status == RequestStatus.Submitted);
                        break;
                    case "inprogress":
                    case "in progress":
                        query = query.Where(r => r.Status == RequestStatus.InProgress);
                        break;
                    case "resolved":
                        query = query.Where(r => r.Status == RequestStatus.Resolved || r.Status == RequestStatus.Closed);
                        break;
                    default:
                        viewModel.Filter = "All";
                        break;
                }

                if (term.Length > 0)
                {
                    var pattern = $"%{term}%";
                    query = query.Where(r => EF.Functions.ILike(r.ReferenceNumber, pattern) ||
                                             EF.Functions.ILike(r.IssueType, pattern) ||
                                             EF.Functions.ILike(r.Location, pattern));
                }

                viewModel.Requests = await query
                    .OrderByDescending(r => r.SubmittedOn)
                    .WithoutPhoto()
                    .ToListAsync(cancellationToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "Could not load the staff dashboard.");
                viewModel.LoadError = "The reports could not be loaded from the database. Please refresh in a moment.";
            }

            return View(viewModel);
        }

        // GET: /Staff/Details/RPT-2026-000123
        [HttpGet]
        public async Task<IActionResult> Details(string id, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }

            var request = await _context.ServiceRequests
                .AsNoTracking()
                .Where(r => r.ReferenceNumber == id)
                .WithoutPhoto()
                .FirstOrDefaultAsync(cancellationToken);

            if (request == null)
            {
                return NotFound();
            }

            ViewBag.Teams = Teams.Contains(request.AssignedTeam) ? Teams : Teams.Append(request.AssignedTeam).ToArray();
            return View(request);
        }

        // POST: /Staff/UpdateStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(
            string referenceNumber, RequestStatus status, PriorityLevel priority,
            string? assignedTeam, string? staffNotes, CancellationToken cancellationToken)
        {
            var request = await _context.ServiceRequests
                .FirstOrDefaultAsync(r => r.ReferenceNumber == referenceNumber, cancellationToken);

            if (request == null)
            {
                return NotFound();
            }

            // Update Status, Priority, Staff Notes, and Assigned Team
            request.Status = status;
            request.Priority = priority;
            request.AssignedTeam = string.IsNullOrWhiteSpace(assignedTeam) ? "Unassigned" : assignedTeam.Trim();
            request.StaffNotes = string.IsNullOrWhiteSpace(staffNotes) ? null : staffNotes.Trim();
            request.UpdatedOn = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync(cancellationToken);
                TempData["SuccessMessage"] = $"Request {referenceNumber} updated successfully.";
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "Could not update request {Reference}.", referenceNumber);
                TempData["ErrorMessage"] = $"Request {referenceNumber} could not be updated. Please try again.";
            }

            return RedirectToAction(nameof(Dashboard));
        }
    }
}
