using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Community_Portal.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Community_Portal.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class AccountController : Controller
    {
        private readonly StaffAccountOptions _staff;

        public AccountController(IOptions<StaffAccountOptions> staffOptions)
        {
            _staff = staffOptions.Value;
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Dashboard", "Staff");
            }

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password, bool rememberMe, string? returnUrl = null)
        {
            var valid = SecureEquals((email ?? string.Empty).Trim().ToLowerInvariant(), _staff.Username.Trim().ToLowerInvariant())
                        & SecureEquals(password ?? string.Empty, _staff.Password);

            if (!valid)
            {
                await Task.Delay(500); // slow down password guessing
                ViewBag.ErrorMessage = "Invalid username/email or password.";
                ViewBag.ReturnUrl = returnUrl;
                return View();
            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, _staff.DisplayName),
                new(ClaimTypes.Email, _staff.Username),
                new(ClaimTypes.Role, "Staff")
            };
            var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties { IsPersistent = rememberMe });

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return LocalRedirect(returnUrl);
            }

            // Authenticate and redirect staff directly to the dashboard
            return RedirectToAction("Dashboard", "Staff");
        }

        // POST: /Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        // Compares via SHA-256 hashes with a fixed-time comparison so response time does not leak information.
        private static bool SecureEquals(string a, string b) =>
            CryptographicOperations.FixedTimeEquals(
                SHA256.HashData(Encoding.UTF8.GetBytes(a)),
                SHA256.HashData(Encoding.UTF8.GetBytes(b)));
    }
}
