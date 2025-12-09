using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace CasinoApp.Domain.Entities
{
    public class User : IdentityUser<int>
    {
        public Wallet Wallet { get; set; }
        public ICollection<Bet> Bets { get; set; } = new List<Bet>();
    }
}