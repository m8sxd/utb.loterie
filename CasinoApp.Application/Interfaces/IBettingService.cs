using CasinoApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CasinoApp.Application.Interfaces
{
    public interface IBettingService
    {
        Task<(Bet? Bet, string Error)> PlaceBetAsync(int userId, decimal stake, List<BetSelection> selections);
        Task<(bool Success, string Error)> SettleBetAsync(Guid betId, bool isWinner);
    }
}