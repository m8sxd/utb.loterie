using CasinoApp.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace CasinoApp.Application.Interfaces
{
    public interface IWalletRepository
    {
        Task<Wallet?> GetByUserIdAsync(int userId);
        Task<Wallet?> GetByIdAsync(Guid walletId);

        Task UpdateAsync(Wallet wallet);

        Task AddTransactionAsync(Transaction transaction);
    }
}