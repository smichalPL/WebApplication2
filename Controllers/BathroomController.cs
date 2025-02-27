using Microsoft.AspNetCore.Mvc;

namespace WebApplication2.Controllers
{
    public class BathroomController : Controller
    {
        public async Task<IActionResult> Index() // Dodajemy async
        {
            return View();
        }
    }
}
