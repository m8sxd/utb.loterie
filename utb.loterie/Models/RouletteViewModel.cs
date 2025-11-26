using System.ComponentModel.DataAnnotations;

namespace utb.loterie.Models
{
    public class RouletteViewModel
    {
        [Required]
        [Range(10, 10000)]
        public decimal Stake { get; set; } = 100;

        [Required]
        public string BetType { get; set; }

        [Required]
        public string BetValue { get; set; }
    }
}