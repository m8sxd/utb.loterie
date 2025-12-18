using System.ComponentModel.DataAnnotations;

namespace utb.loterie.Models
{
    public class SlotsViewModel
    {
        [Required]

        public decimal Stake { get; set; }
    }
}