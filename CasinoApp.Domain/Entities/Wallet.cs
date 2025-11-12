using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CasinoApp.Domain.Entities
{
    public class Wallet
    {
        public Guid Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        [MaxLength(3)]
        public string Currency { get; set; } = "EUR";

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Balance { get; set; } = 0.00m;

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Reserved { get; set; } = 0.00m;

        [Timestamp]
        public byte[] ConcurrencyToken { get; set; } = new byte[0];

        public void UpdateBalance(decimal amount, bool isReserved)
        {
            if (isReserved)
            {
                Balance -= amount;
                Reserved += amount;
            }
            else
            {
                Balance += amount;
            }
        }

        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}