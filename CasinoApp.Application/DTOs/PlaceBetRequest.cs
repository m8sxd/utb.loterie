using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CasinoApp.Application.DTOs
{
    public class PlaceBetRequest
    {
        [Required]
        public int UserId { get; set; } 

        [Required]
        [Range(0.01, 1000000.0)]
        public decimal Stake { get; set; }

        [Required]
        [MinLength(1)]
        public List<BetSelectionDTO> Selections { get; set; } = new List<BetSelectionDTO>();
    }

    public class BetSelectionDTO
    {
        [Required]
        public Guid MarketId { get; set; }
        [Required]
        public Guid OddsId { get; set; }
        [Required]
        public string Selection { get; set; }
        [Required]
        public decimal DecimalOdds { get; set; }
    }
}