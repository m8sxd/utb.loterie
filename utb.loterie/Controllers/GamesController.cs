using Microsoft.AspNetCore.Mvc;

namespace utb.loterie.Controllers 
{
    public class GamesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        
        public IActionResult Roulette()
        {
            return View();
        }
        
    }
}