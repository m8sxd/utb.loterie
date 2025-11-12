using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CasinoApp.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }

        [MaxLength(100)]
        public string Username { get; set; }

        [MaxLength(255)]
        public string Email { get; set; }

        public string PasswordHash { get; set; }

        public Wallet Wallet { get; set; }
        public ICollection<Bet> Bets { get; set; } = new List<Bet>();
    }
}