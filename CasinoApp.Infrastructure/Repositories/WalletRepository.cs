using CasinoApp.Application.Interfaces;
using CasinoApp.Domain.Entities;
using CasinoApp.Infrastructure.Database; // ZMĚNA
using Microsoft.EntityFrameworkCore;

namespace CasinoApp.Infrastructure.Repositories
{
    public class WalletRepository : IWalletRepository
    {
        private readonly AppDbContext _context;

        public WalletRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Wallet?> GetByUserIdAsync(Guid userId)
        {
            return await _context.Wallets
                .FirstOrDefaultAsync(w => w.UserId == userId);
        }

        public async Task<Wallet?> GetByIdAsync(Guid walletId)
        {
            return await _context.Wallets
                .FirstOrDefaultAsync(w => w.Id == walletId);
        }

        public Task UpdateAsync(Wallet wallet)
        {
            _context.Wallets.Update(wallet);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}