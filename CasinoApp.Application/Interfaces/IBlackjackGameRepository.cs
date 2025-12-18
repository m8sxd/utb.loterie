using CasinoApp.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace CasinoApp.Application.Interfaces
{
    public interface IBlackjackGameRepository
    {
        Task AddAsync(BlackjackGame game);

        Task<BlackjackGame> GetByIdAndUserIdAsync(Guid gameId, int userId);

        Task UpdateAsync(BlackjackGame game);
    }
}