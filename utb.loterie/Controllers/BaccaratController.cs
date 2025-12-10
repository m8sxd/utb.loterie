using CasinoApp.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using utb.loterie.Filters;

namespace CasinoApp.Controllers
{
    [Authorize]
    [GameBanFilter]
    public class BaccaratController : Controller
    {
        private readonly BaccaratService _baccaratService;

        public BaccaratController(BaccaratService baccaratService)
        {
            _baccaratService = baccaratService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Play([FromBody] BaccaratRequest request)
        {
            if (request.Stake <= 0) return BadRequest("Špatná sázka");

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            try
            {
                var result = await _baccaratService.PlayRoundAsync(userId, request.Stake, request.BetType);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }

    public class BaccaratRequest
    {
        public decimal Stake { get; set; }
        public string BetType { get; set; } 
    }
}