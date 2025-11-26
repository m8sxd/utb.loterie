using System.ComponentModel.DataAnnotations;

namespace utb.loterie.Models
{
    public class DiceViewModel
    {
        [Required]
        [Range(10, 10000, ErrorMessage = "Sázka musí být mezi 10 a 10 000 Kč.")]
        public decimal Stake { get; set; } = 100;

        [Required]
        [Range(1, 6, ErrorMessage = "Musíte vybrat číslo 1 až 6.")]
        public int Guess { get; set; } = 6; // Výchozí tip
    }
}