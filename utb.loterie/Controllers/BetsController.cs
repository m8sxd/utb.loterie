using CasinoApp.Application.Interfaces;
using CasinoApp.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace utb.loterie.Controllers
{
    // POZNÁMKA: Pro produkční aplikaci byste zde používali DTOs (Data Transfer Objects),
    // ne přímo doménové entity (Bet, BetSelection). Pro zjednodušení použijeme entity.

    [ApiController]
    [Route("api/[controller]")]
    public class BetsController : ControllerBase
    {
        private readonly IBettingService _bettingService;

        public BetsController(IBettingService bettingService)
        {
            _bettingService = bettingService;
        }

        /// <summary>
        /// Umístí novou sázku a provede atomickou finanční transakci (odečtení ze zůstatku).
        /// </summary>
        /// <param name="request">Obsahuje UserID, výši sázky (Stake) a seznam vybraných tipů (Selections).</param>
        /// <returns>Nově vytvořená sázka nebo chybová zpráva.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PlaceBet([FromBody] PlaceBetRequest request)
        {
            // Zde by měla proběhnout validace requestu (ModelState.IsValid)
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Volání aplikační služby pro umístění sázky
            var (bet, error) = await _bettingService.PlaceBetAsync(
                request.UserId,
                request.Stake,
                request.Selections
            );

            if (bet != null)
            {
                // Vracíme HTTP 201 Created s objektem sázky
                return CreatedAtAction(nameof(PlaceBet), new { id = bet.Id }, bet);
            }
            else
            {
                // Vracíme HTTP 400 Bad Request s chybovou zprávou (např. Nedostatečný zůstatek)
                return BadRequest(new { Message = error });
            }
        }
    }

    // Třída pro příjem dat z HTTP požadavku (v ideálním případě by byla v DTO složce)
    public class PlaceBetRequest
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        [Range(0.01, 1000000.0)]
        public decimal Stake { get; set; }

        [Required]
        [MinLength(1)]
        public List<BetSelection> Selections { get; set; } = new List<BetSelection>();
    }
}