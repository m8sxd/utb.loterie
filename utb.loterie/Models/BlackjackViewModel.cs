using System;
using System.ComponentModel.DataAnnotations;

namespace utb.loterie.Models
{

    public class BlackjackStartViewModel
    {
        [Required(ErrorMessage = "Zadejte částku sázky.")]
        [Range(10, 10000, ErrorMessage = "Sázka musí být mezi 10 a 10000 Kč.")]
        public decimal Stake { get; set; }
    }


    public class BlackjackActionViewModel
    {
        [Required]
        public Guid GameId { get; set; }
    }
}