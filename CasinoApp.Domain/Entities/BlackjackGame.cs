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

        // STAV HRY:
        // "PlayerTurn" - Čeká se, jestli hráč dá Hit nebo Stand.
        // "DealerTurn" - Hráč dal Stand, dealer hraje svou logiku.
        // "Finished"   - Hra je u konce (vyhodnocená).
        [MaxLength(20)]
        public string Status { get; set; } = "PlayerTurn";

        // KARTY:
        // Uložíme je jako jednoduchý řetězec (string), např. "S-10,H-A,D-2".
        // Před použitím v C# je rozparsujeme zpět na objekty Card.
        public string PlayerHand { get; set; }
        public string DealerHand { get; set; }

        // Balíček karet - musíme si pamatovat, které karty už byly rozdány
        public string Deck { get; set; }

        // Časová razítka pro audit
        public DateTime StartTime { get; set; } = DateTime.UtcNow;
        public DateTime? EndTime { get; set; }

        // Volitelně: Výsledek hry pro rychlé zobrazení v historii
        public string? ResultMessage { get; set; } 
    }
}