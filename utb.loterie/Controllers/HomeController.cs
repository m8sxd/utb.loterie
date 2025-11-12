using Microsoft.AspNetCore.Mvc;

namespace utb.loterie.Controllers
{
    // Toto je MVC kontroler, který vrací Views
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            // Toto vrátí soubor /Views/Home/Index.cshtml
            return View();
        }

        // Pro budoucí chybovou stránku
        public IActionResult Error()
        {
            return View();
        }
    }
}