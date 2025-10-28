using CasinoApp.Domain.Entities;
using CasinoApp.Application.Interfaces;

namespace CasinoApp.Application.Services
{
    public class WalletService
    {
        private readonly IWalletRepository _walletRepository;

        public WalletService(IWalletRepository walletRepository)
        {
            _walletRepository = walletRepository;
        }
        public async Task<bool> ProcessDepositAsync(Guid userId, decimal amount)
        {
            if (amount <= 0) return false;

            var wallet = await _walletRepository.GetByUserIdAsync(userId);
            if (wallet == null) return false;

            wallet.Balance += amount;

            await _walletRepository.UpdateAsync(wallet);
            await _walletRepository.SaveChangesAsync();

            return true;
        }
    }
}