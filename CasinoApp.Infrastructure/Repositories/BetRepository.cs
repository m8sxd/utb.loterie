using CasinoApp.Application.Interfaces;
using CasinoApp.Domain.Entities;
using CasinoApp.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace CasinoApp.Infrastructure.Repositories
{
    public class BetRepository : IBetRepository
    {
        private readonly AppDbContext _context;

        public BetRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Bet?> GetByIdWithWalletAsync(Guid betId)
        {
            return await _context.Bets
                .Include(b => b.Wallet)
                .FirstOrDefaultAsync(b => b.Id == betId);
        }
    }
}