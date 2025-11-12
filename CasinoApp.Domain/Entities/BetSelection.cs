using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CasinoApp.Domain.Entities
{
    public class BetSelection
    {
        public Guid Id { get; set; }

        public Guid BetId { get; set; }
        public Bet Bet { get; set; }

        public Guid MarketId { get; set; }

        public Guid OddsId { get; set; }

        [MaxLength(255)]
        public string Selection { get; set; }

        [Column(TypeName = "decimal(10, 4)")]
        public decimal DecimalOdds { get; set; }

        [MaxLength(20)]
        public string SettlementStatus { get; set; } = "Pending";
    }
}