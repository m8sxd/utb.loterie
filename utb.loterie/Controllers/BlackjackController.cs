using CasinoApp.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using utb.loterie.Models;

namespace CasinoApp.Controllers
{
    [Authorize]
    public class BlackjackController : Controller
    {
        private readonly IGameService _gameService;

        public BlackjackController(IGameService gameService)
        {
            _gameService = gameService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Start([FromBody] BlackjackStartViewModel model)
        {
            if (!ModelState.IsValid) return BadRequest("Neplatná sázka.");

            try
            {
                int userId = GetCurrentUserId();
                var gameState = await _gameService.StartBlackjackAsync(userId, model.Stake);
                return Ok(gameState);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Hit([FromBody] BlackjackActionViewModel model)
        {
            if (!ModelState.IsValid) return BadRequest();

            try
            {
                int userId = GetCurrentUserId();
                var gameState = await _gameService.BlackjackHitAsync(model.GameId, userId);
                return Ok(gameState);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Stand([FromBody] BlackjackActionViewModel model)
        {
            if (!ModelState.IsValid) return BadRequest();

            try
            {
                int userId = GetCurrentUserId();
                var gameState = await _gameService.BlackjackStandAsync(model.GameId, userId);
                return Ok(gameState);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        private int GetCurrentUserId()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userIdStr, out int userId)) return userId;
            throw new Exception("Uživatel není přihlášen.");
        }
    }
}