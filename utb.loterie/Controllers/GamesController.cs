using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;

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
        
        [HttpPost]
        public IActionResult SpinRoulette(/* Data o sázkách    TO DO! */)
        {

            int winningNumber = RandomNumberGenerator.GetInt32(0, 37);

            // Logika pro výpočet výhry a uložení do DB     TO DO!
            
            return Json(new { winningNumber = winningNumber });
        }
        
    }
}