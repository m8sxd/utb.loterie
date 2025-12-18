using CasinoApp.Application.DTOs; 
using CasinoApp.Application.Interfaces;
using CasinoApp.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace utb.loterie.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BetsController : ControllerBase
    {
        private readonly IBettingService _bettingService;

        public BetsController(IBettingService bettingService)
        {
            _bettingService = bettingService;
        }

        [HttpPost]
        public async Task<IActionResult> PlaceBet([FromBody] PlaceBetRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var selections = request.Selections.Select(s => new BetSelection
            {
                MarketId = s.MarketId,
                OddsId = s.OddsId,
                Selection = s.Selection,
                DecimalOdds = s.DecimalOdds
            }).ToList();

            var (bet, error) = await _bettingService.PlaceBetAsync(
                request.UserId,
                request.Stake,
                selections
            );

            if (bet != null)
            {
                return CreatedAtAction(nameof(PlaceBet), new { id = bet.Id }, bet);
            }
            else
            {
                return BadRequest(new { Message = error });
            }
        }
    }
}