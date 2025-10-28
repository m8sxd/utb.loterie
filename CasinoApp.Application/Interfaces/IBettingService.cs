using CasinoApp.Domain.Entities;

namespace CasinoApp.Application.Interfaces
{
    public interface IBettingService
    {
        Task<(Bet? Bet, string Error)> PlaceBetAsync(Guid userId, decimal stake, List<BetSelection> selections);
        Task<bool> SettleBetAsync(Guid betId, string finalOutcome);
    }
}