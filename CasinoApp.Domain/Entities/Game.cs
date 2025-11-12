using System;
using System.ComponentModel.DataAnnotations;

namespace CasinoApp.Domain.Entities
{
    public class Game
    {
        public Guid Id { get; set; }

        [MaxLength(100)]
        public string Code { get; set; }

        [MaxLength(255)]
        public string Name { get; set; }

        [MaxLength(50)]
        public string GameType { get; set; }
    }
}