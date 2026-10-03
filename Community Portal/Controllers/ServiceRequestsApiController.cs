/* Microsoft. 2026. Create web APIs with ASP.NET Core. [Online].
 * Available at: <https://learn.microsoft.com/en-us/aspnet/core/web-api/>
 * [Accessed 3 October 2026].*/

using System.ComponentModel.DataAnnotations;
using Community_Portal.Data;
using Community_Portal.Models;
using Community_Portal.Services;
using CommunityPortal.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Community_Portal.Controllers
{
    /// <summary>Public JSON API documented in Swagger (/swagger). The web pages use the MVC controllers instead.</summary>
    [ApiController]
    [Route("api/service-requests")]
    public class ServiceRequestsApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ServiceRequestsApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        public record CreateRequest(
            [Required, StringLength(100)] string IssueType,
            [Required, StringLength(2000, MinimumLength = 10)] string Description,
            [Required, StringLength(250)] string Location);

        public record RequestStatusResponse(
            string ReferenceNumber, string IssueType, string Location, string Description,
            string Status, string Priority, string AssignedTeam, DateTime SubmittedOn, DateTime? UpdatedOn);

        /// <summary>Look up the public status of a report by its reference number.</summary>
        [HttpGet("{referenceNumber}")]
        [ProducesResponseType(typeof(RequestStatusResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RequestStatusResponse>> Get(string referenceNumber, CancellationToken cancellationToken)
        {
            var key = referenceNumber.Trim().ToUpperInvariant();
            var request = await _context.ServiceRequests.AsNoTracking()
                .Where(r => r.ReferenceNumber == key)
                .WithoutPhoto()
                .FirstOrDefaultAsync(cancellationToken);

            return request is null ? NotFound() : ToResponse(request);
        }

        /// <summary>Submit a new service request.</summary>
        [HttpPost]
        [ProducesResponseType(typeof(RequestStatusResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<RequestStatusResponse>> Create(CreateRequest body, CancellationToken cancellationToken)
        {
            var request = new ServiceRequest
            {
                ReferenceNumber = ReferenceGenerator.Generate(),
                IssueType = body.IssueType.Trim(),
                Description = body.Description.Trim(),
                Location = body.Location.Trim(),
                Status = RequestStatus.Submitted,
                Priority = PriorityLevel.Medium,
                SubmittedOn = DateTime.UtcNow
            };

            _context.ServiceRequests.Add(request);
            await _context.SaveChangesAsync(cancellationToken);

            return CreatedAtAction(nameof(Get), new { referenceNumber = request.ReferenceNumber }, ToResponse(request));
        }

        private static RequestStatusResponse ToResponse(ServiceRequest r) =>
            new(r.ReferenceNumber, r.IssueType, r.Location, r.Description,
                r.StatusDisplayName, r.Priority.ToString(), r.AssignedTeam, r.SubmittedOn, r.UpdatedOn);
    }
}
