using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CasinoApp.Domain.Entities
{
    public class Bet
    {
        public Guid Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public Guid WalletId { get; set; }
        public Wallet Wallet { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Stake { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal PotentialPayout { get; set; }

        [MaxLength(32)]
        public string Status { get; set; } = "Pending";

        public DateTime PlacedAt { get; set; } = DateTime.UtcNow;
        public DateTime? SettledAt { get; set; }
        public ICollection<BetSelection> Selections { get; set; } = new List<BetSelection>();
    }
}