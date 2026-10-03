/* Microsoft. 2026. ASP.NET Core MVC Controllers and Action Results Overview. [Online].
 * Available at: <https://learn.microsoft.com/en-us/aspnet/core/mvc/controllers/actions>
 * [Accessed 2 October 2026].
 *
 * Microsoft. 2026. Upload files in ASP.NET Core. [Online].
 * Available at: <https://learn.microsoft.com/en-us/aspnet/core/mvc/models/file-uploads>
 * [Accessed 3 October 2026].*/

using Community_Portal.Data;
using Community_Portal.Models;
using Community_Portal.Services;
using CommunityPortal.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Community_Portal.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ServiceRequestController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ServiceRequestController> _logger;

        // Inject the database context so the controller can talk to Supabase
        public ServiceRequestController(ApplicationDbContext context, ILogger<ServiceRequestController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: /ServiceRequest/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View(new CreateServiceRequestViewModel());
        }

        // POST: /ServiceRequest/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(8 * 1024 * 1024)]
        public async Task<IActionResult> Create(CreateServiceRequestViewModel model, CancellationToken cancellationToken)
        {
            if (!CreateServiceRequestViewModel.IssueTypes.Contains(model.IssueType))
            {
                ModelState.AddModelError(nameof(model.IssueType), "Please select an issue type from the list.");
            }

            // Optional photo: validated by size and by its real file signature.
            byte[]? photoBytes = null;
            string? photoContentType = null;
            if (model.Photo is { Length: > 0 })
            {
                if (model.Photo.Length > ImageValidator.MaxBytes)
                {
                    ModelState.AddModelError(nameof(model.Photo), "The photo must be smaller than 4 MB.");
                }
                else
                {
                    using var buffer = new MemoryStream();
                    await model.Photo.CopyToAsync(buffer, cancellationToken);
                    photoBytes = buffer.ToArray();
                    photoContentType = ImageValidator.DetectContentType(photoBytes);

                    if (photoContentType is null)
                    {
                        ModelState.AddModelError(nameof(model.Photo), "Please upload a JPEG, PNG, GIF or WebP image.");
                    }
                }
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // Generate a reference number and make sure it is not already in use.
                var referenceNumber = ReferenceGenerator.Generate();
                for (var attempt = 0; attempt < 5 &&
                     await _context.ServiceRequests.AnyAsync(r => r.ReferenceNumber == referenceNumber, cancellationToken); attempt++)
                {
                    referenceNumber = ReferenceGenerator.Generate();
                }

                // Create the model object to store in Supabase
                var serviceRequest = new ServiceRequest
                {
                    ReferenceNumber = referenceNumber,
                    IssueType = model.IssueType,
                    Description = model.Description.Trim(),
                    Location = model.Location.Trim(),
                    Status = RequestStatus.Submitted,
                    Priority = PriorityLevel.Medium,
                    SubmittedOn = DateTime.UtcNow,
                    AssignedTeam = "Unassigned",
                    HasPhoto = photoBytes is not null,
                    PhotoData = photoBytes,
                    PhotoContentType = photoContentType
                };

                // Add to database context and save changes asynchronously
                _context.ServiceRequests.Add(serviceRequest);
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Service request {Reference} created.", referenceNumber);

                TempData["SuccessMessage"] = "Your report has been submitted. Keep your reference number to track its progress.";
                return RedirectToAction(nameof(Track), new { referenceNumber });
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "Could not save a service request.");
                ModelState.AddModelError(string.Empty,
                    "We could not save your report right now. Please try again in a moment.");
                return View(model);
            }
        }

        // GET: /ServiceRequest/Track?referenceNumber=RPT-2026-123456
        [HttpGet]
        public async Task<IActionResult> Track(string? referenceNumber, CancellationToken cancellationToken)
        {
            var viewModel = new TrackViewModel { Query = referenceNumber?.Trim() };

            if (viewModel.Searched)
            {
                var lookup = viewModel.Query!.ToUpperInvariant();
                try
                {
                    viewModel.Request = await _context.ServiceRequests
                        .AsNoTracking()
                        .Where(r => r.ReferenceNumber == lookup)
                        .WithoutPhoto()
                        .FirstOrDefaultAsync(cancellationToken);
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    _logger.LogError(ex, "Tracking lookup failed for {Reference}.", lookup);
                    viewModel.Error = "We could not look up your report right now. Please try again shortly.";
                }
            }

            return View(viewModel);
        }

        // GET: /ServiceRequest/Photo/RPT-2026-123456  (municipality staff only)
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Photo(string id, CancellationToken cancellationToken)
        {
            var photo = await _context.ServiceRequests
                .AsNoTracking()
                .Where(r => r.ReferenceNumber == id && r.HasPhoto)
                .Select(r => new { r.PhotoData, r.PhotoContentType })
                .FirstOrDefaultAsync(cancellationToken);

            if (photo?.PhotoData is null)
            {
                return NotFound();
            }

            Response.Headers["X-Content-Type-Options"] = "nosniff";
            return File(photo.PhotoData, photo.PhotoContentType ?? "application/octet-stream");
        }
    }
}
