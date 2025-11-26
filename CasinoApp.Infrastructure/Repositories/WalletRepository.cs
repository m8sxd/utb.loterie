using CasinoApp.Application.Interfaces;
using CasinoApp.Domain.Entities;
using CasinoApp.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace CasinoApp.Infrastructure.Repositories
{
    public class WalletRepository : IWalletRepository
    {
        private readonly AppDbContext _context;

        public WalletRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Wallet?> GetByIdAsync(Guid walletId)
        {
            return await _context.Wallets
                .AsNoTracking() 
                .FirstOrDefaultAsync(w => w.Id == walletId);
        }

        public async Task<Wallet?> GetByUserIdAsync(int userId)
        {
            return await _context.Wallets
                .AsNoTracking() 
                .Include(w => w.Transactions)
                .FirstOrDefaultAsync(w => w.UserId == userId);
        }

        public async Task UpdateAsync(Wallet wallet)
        {
            await _context.Database.ExecuteSqlInterpolatedAsync(
                $"UPDATE Wallets SET Balance = {wallet.Balance}, Reserved = {wallet.Reserved} WHERE Id = {wallet.Id}"
            );
        }

        public async Task AddTransactionAsync(Transaction transaction)
        {
            await _context.Transactions.AddAsync(transaction);
        }
    }
}