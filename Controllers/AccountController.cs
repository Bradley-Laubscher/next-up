using Microsoft.AspNetCore.Mvc;

namespace NextUp.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Logout()
        {
            // Implement logout logic if necessary
            return RedirectToAction("Login");
        }
    }
}