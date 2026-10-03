using Community_Portal.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Community_Portal.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // Re-executed by UseStatusCodePagesWithReExecute for 404, 403, etc.
        [Route("Home/HttpStatus")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult HttpStatus(int code = 404)
        {
            Response.StatusCode = code;
            ViewBag.Code = code;
            return View("StatusPage");
        }
    }
}
