using Microsoft.AspNetCore.Mvc;

namespace TechWiz.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult DashBoard()
        {
            return View();
        }
    }
}
