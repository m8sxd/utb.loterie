using System.ComponentModel.DataAnnotations;

namespace utb.loterie.Models
{
    // Model pro data, která posílá JavaScript při zatočení slotů
    public class SlotsViewModel
    {
        [Required]

        public decimal Stake { get; set; }
    }
}