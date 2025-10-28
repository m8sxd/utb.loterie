using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CasinoApp.Domain.Entities
{
    public class Transaction
    {
        public Guid Id { get; set; }

        public Guid WalletId { get; set; }
        public Wallet Wallet { get; set; }

        [MaxLength(32)]
        public string Type { get; set; } = string.Empty;

        public decimal Amount { get; set; }

        public decimal BalanceAfter { get; set; }

        public Guid? ReferenceId { get; set; }

        public string? Note { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(ReferenceId))]
        public Bet? Bet { get; set; }
    }
}