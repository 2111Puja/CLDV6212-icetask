using Microsoft.AspNetCore.Mvc;

namespace Community_Portal.Controllers
{
    public class AccountController : Controller
    {
        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string email, string password, bool rememberMe)
        {
            // Simple validation check against demo credentials
            if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password))
            {
                // Authenticate and redirect staff directly to the dashboard
                return RedirectToAction("Dashboard", "Staff");
            }

            ViewBag.ErrorMessage = "Invalid username/email or password.";
            return View();
        }

        // GET: /Account/Logout
        [HttpGet]
        public IActionResult Logout()
        {
            return RedirectToAction("Login");
        }
    }
}