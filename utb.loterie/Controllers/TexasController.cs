using CasinoApp.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CasinoApp.Controllers
{
    [Authorize]
    public class TexasController : Controller
    {
        private readonly TexasHoldemService _service;

        public TexasController(TexasHoldemService service)
        {
            _service = service;
        }

        public IActionResult Index() => View();

        [HttpPost]
        public async Task<IActionResult> Start([FromBody] TexasRequest req)
        {
            try
            {
                int uid = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var res = await _service.StartGameAsync(uid, req.Stake, HttpContext.Session);
                return Ok(res);
            }
            catch (Exception e) { return BadRequest(new { message = e.Message }); }
        }

        [HttpPost]
        public async Task<IActionResult> Action([FromBody] TexasActionReq req)
        {
            try
            {
                int uid = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var res = await _service.NextStepAsync(uid, req.Action, HttpContext.Session);
                return Ok(res);
            }
            catch (Exception e) { return BadRequest(new { message = e.Message }); }
        }

        [HttpPost]
        public IActionResult Fold()
        {
            _service.Fold(HttpContext.Session);
            return Ok(new { message = "Složil jsi karty." });
        }
    }

    public class TexasRequest { public decimal Stake { get; set; } }
    public class TexasActionReq { public string Action { get; set; } }
}