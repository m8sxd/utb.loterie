using CasinoApp.Application.Interfaces;
using CasinoApp.Domain.Entities;
using System.Threading.Tasks;

namespace CasinoApp.Application.Services
{
    public class WalletService
    {
        private readonly IWalletRepository _walletRepository;

        public WalletService(IWalletRepository walletRepository)
        {
            _walletRepository = walletRepository;
        }

        public async Task<Wallet?> GetWalletForUser(int userId)
        {
            return await _walletRepository.GetByUserIdAsync(userId);
        }

    }
}