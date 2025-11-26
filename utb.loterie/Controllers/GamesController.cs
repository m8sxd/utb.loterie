using CasinoApp.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Cryptography;
using utb.loterie.Models;

namespace utb.loterie.Controllers
{
    [Authorize]
    public class GamesController : Controller
    {
        private readonly IGameService _gameService;

        public GamesController(IGameService gameService)
        {
            _gameService = gameService;
        }

        public IActionResult Index()
        {
            return View();
        }

        // --- RULETA (IMPLEMENTOVÁNO) ---

        public IActionResult Roulette()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SpinRoulette([FromBody] RouletteViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new { message = "Neplatná sázka." });

                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!int.TryParse(userIdString, out int userId))
                    return Unauthorized(new { message = "Nejste přihlášen." });

                var result = await _gameService.PlayRouletteAsync(userId, model.Stake, model.BetType, model.BetValue);

                if (string.IsNullOrEmpty(result.Message))
                    return BadRequest(new { message = "Chyba transakce (nedostatek peněz?)." });

                return Json(new
                {
                    winningNumber = result.RolledNumber,
                    isWin = result.IsWin,
                    winAmount = result.WinAmount,
                    balance = result.NewBalance,
                    message = result.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // --- KOSTKY / DICE (IMPLEMENTOVÁNO) ---

        [HttpGet]
        public IActionResult Dice()
        {
            return View(new DiceViewModel { Stake = 100, Guess = 6 });
        }

        [HttpPost]
        public async Task<IActionResult> PlayDice([FromBody] DiceViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new { message = "Neplatná sázka." });

                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!int.TryParse(userIdString, out int userId))
                    return Unauthorized(new { message = "Nejste přihlášen." });

                var result = await _gameService.PlayDiceAsync(userId, model.Stake, model.Guess);

                if (string.IsNullOrEmpty(result.Message))
                    return BadRequest(new { message = "Chyba transakce (nedostatek peněz?)." });

                return Json(new
                {
                    rolled = result.RolledNumber,
                    isWin = result.IsWin,
                    winAmount = result.WinAmount,
                    balance = result.NewBalance,
                    message = result.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}