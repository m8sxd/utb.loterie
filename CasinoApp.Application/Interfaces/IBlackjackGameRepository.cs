using CasinoApp.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace CasinoApp.Application.Interfaces
{
    // Definice metod, které potřebujeme pro práci s Blackjack hrami v DB
    public interface IBlackjackGameRepository
    {
        // Přidá novou rozehranou hru
        Task AddAsync(BlackjackGame game);

        // Najde konkrétní hru podle jejího ID a ID hráče.
        // DŮLEŽITÉ: Musí načíst (Include) i připojeného Uživatele a jeho Peněženku,
        // protože je budeme potřebovat pro aktualizaci zůstatku na konci hry.
        Task<BlackjackGame> GetByIdAndUserIdAsync(Guid gameId, int userId);

        // Uloží změny ve stavu hry (např. po Hitu nebo Standu)
        Task UpdateAsync(BlackjackGame game);
    }
}