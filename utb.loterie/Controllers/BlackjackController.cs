using CasinoApp.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using utb.loterie.Models; // Import našich nových ViewModelů

namespace utb.loterie.Controllers
{
    [Authorize] // Blackjack mohou hrát jen přihlášení
    public class BlackjackController : Controller
    {
        private readonly IGameService _gameService;

        // Injectujeme naši službu, která obsahuje veškerou logiku
        public BlackjackController(IGameService gameService)
        {
            _gameService = gameService;
        }

        // GET: /Blackjack
        // Zobrazí prázdnou herní stránku
        public IActionResult Index()
        {
            return View();
        }

        // ================= API ENDPOINTY PRO JS =================

        // POST: /Blackjack/Start
        [HttpPost]
        public async Task<IActionResult> Start([FromBody] BlackjackStartViewModel model)
        {
            if (!ModelState.IsValid) return BadRequest("Neplatná sázka.");

            try
            {
                int userId = GetCurrentUserId();
                // Voláme službu a získáme úvodní stav hry
                var gameState = await _gameService.StartBlackjackAsync(userId, model.Stake);
                return Ok(gameState); // Vracíme JSON se stavem
            }
            catch (Exception ex)
            {
                // Zachytíme chyby (např. nedostatek peněz) a pošleme je frontendu
                return BadRequest(new { message = ex.Message });
            }
        }

        // POST: /Blackjack/Hit
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

        // POST: /Blackjack/Stand
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

        // Pomocná metoda pro získání ID přihlášeného uživatele
        private int GetCurrentUserId()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userIdStr, out int userId)) return userId;
            throw new Exception("Uživatel není přihlášen.");
        }
    }
}