/* Microsoft. 2026. ASP.NET Core MVC Controllers and Action Results Overview. [Online].
 * Available at: <https://learn.microsoft.com/en-us/aspnet/core/mvc/controllers/actions>
 * [Accessed 2 October 2026].*/

using Community_Portal.Data;
using Community_Portal.Models;
using CommunityPortal.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Community_Portal.Controllers
{
    public class ServiceRequestController : Controller
    {
        private readonly ApplicationDbContext _context;

        // Inject the database context so the controller can talk to Supabase
        public ServiceRequestController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /ServiceRequest/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: /ServiceRequest/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string issueType, string description, string location)
        {
            // Generate a demo reference number for testing
            string generatedReference = "RPT-2026-" + new Random().Next(100000, 999999);

            // Create the model object to store in Supabase
            var serviceRequest = new ServiceRequest
            {
                // Note: Adjust property names if they differ slightly in your ServiceRequest.cs model file
                IssueType = issueType,
                Description = description,
                Location = location,
                ReferenceNumber = generatedReference
            };

            // Add to database context and save changes asynchronously
            _context.ServiceRequests.Add(serviceRequest);
            await _context.SaveChangesAsync();

            // TempData survives the redirect below (ViewBag does not), so the Track page can show the confirmation
            TempData["SuccessMessage"] = "Report submitted successfully! Keep your reference number to follow its progress.";

            // Redirect directly to the tracking page with the new reference number
            return RedirectToAction("Track", new { referenceNumber = generatedReference });
        }

        // GET: /ServiceRequest/Track
        [HttpGet]
        public async Task<IActionResult> Track(string? referenceNumber)
        {
            // Pass the reference number to Track.cshtml so the search box keeps what was typed
            var searchedReference = referenceNumber?.Trim();
            ViewData["ReferenceNumber"] = searchedReference;

            ServiceRequest? serviceRequest = null;

            if (!string.IsNullOrEmpty(searchedReference))
            {
                // References are stored in upper case (RPT-2026-123456), so make the lookup case-insensitive
                var lookup = searchedReference.ToUpperInvariant();

                try
                {
                    // Load the exact report the resident submitted from the database
                    serviceRequest = await _context.ServiceRequests
                        .AsNoTracking()
                        .FirstOrDefaultAsync(r => r.ReferenceNumber == lookup);
                }
                catch (Exception)
                {
                    // Show a friendly message on the page instead of a 500 error
                    ViewData["LookupError"] = "We could not look up your report right now. Please try again in a moment.";
                }
            }

            // Null when nothing was searched for, or when no report matches the reference number
            return View(serviceRequest);
        }
    }
}
