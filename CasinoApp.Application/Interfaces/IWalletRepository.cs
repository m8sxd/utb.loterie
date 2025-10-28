using CasinoApp.Domain.Entities;

namespace CasinoApp.Application.Interfaces
{
    public interface IWalletRepository
    {
        Task<Wallet?> GetByUserIdAsync(Guid userId);
        Task<Wallet?> GetByIdAsync(Guid walletId);
        Task UpdateAsync(Wallet wallet);
        Task SaveChangesAsync();
    }
}
