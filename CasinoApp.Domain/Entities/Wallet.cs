using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CasinoApp.Domain.Entities
{
    public class Wallet
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

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
                // Přesun z Balance do Reserved (pro sázky)
                Balance -= amount;
                Reserved += amount;
            }
            else
            {
                // Přímá změna zůstatku (deposit/výplata)
                Balance += amount;
            }
        }
        
        // Navigační property (volitelné, ale dobré)
        // public User User { get; set; }
        // public ICollection<Transaction> Transactions { get; set; } 
    }
}