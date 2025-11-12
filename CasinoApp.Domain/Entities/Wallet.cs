using System.ComponentModel.DataAnnotations;

namespace CasinoApp.Domain.Entities
{
    public class Wallet
    {
        public Guid Id { get; set; }

        public int UserId { get; set; } 
        public User User { get; set; }

        [MaxLength(3)]
        public string Currency { get; set; } = "EUR";

        public decimal Balance { get; set; } = 0.00m;

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
    }
}