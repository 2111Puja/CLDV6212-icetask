/* Microsoft. 2026. ASP.NET Core MVC Controllers and Action Results Overview. [Online].
 * Available at: <https://learn.microsoft.com/en-us/aspnet/core/mvc/controllers/actions>
 * [Accessed 2 October 2026].*/

using Microsoft.AspNetCore.Mvc;

namespace Community_Portal.Controllers
{
    public class ServiceRequestController : Controller
    {
        // GET: /ServiceRequest/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: /ServiceRequest/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(string issueType, string description, string location)
        {
            // Generate a demo reference number for testing
            string generatedReference = "RPT-2026-" + new Random().Next(100000, 999999);

            // Pass confirmation message and reference number to the view
            ViewBag.Message = "Report submitted successfully!";
            ViewBag.ReferenceNumber = generatedReference;

            // Redirect directly to the tracking page with the new reference number
            return RedirectToAction("Track", new { referenceNumber = generatedReference });
        }

        // GET: /ServiceRequest/Track
        [HttpGet]
        public IActionResult Track(string referenceNumber)
        {
            // Pass the reference number to Track.cshtml
            ViewData["ReferenceNumber"] = referenceNumber;
            return View();
        }
    }
}
