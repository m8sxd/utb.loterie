using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CasinoApp.Domain.Entities
{

    public class BlackjackGame
    {

        public Guid Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
        
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Stake { get; set; }

        [MaxLength(20)]
        public string Status { get; set; } = "PlayerTurn";

        public string PlayerHand { get; set; }
        public string DealerHand { get; set; }

        public string Deck { get; set; }

        public DateTime StartTime { get; set; } = DateTime.UtcNow;
        public DateTime? EndTime { get; set; }
        public string? ResultMessage { get; set; } 
    }
}