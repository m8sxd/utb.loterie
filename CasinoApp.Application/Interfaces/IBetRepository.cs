using CasinoApp.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace CasinoApp.Application.Interfaces
{
    public interface IBetRepository
    {
        Task<Bet?> GetByIdWithWalletAsync(Guid betId);
    }
}