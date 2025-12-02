using CasinoApp.Application.Interfaces;
using CasinoApp.Domain.Entities;
using CasinoApp.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace CasinoApp.Infrastructure.Repositories
{
    public class BlackjackGameRepository : IBlackjackGameRepository
    {
        private readonly AppDbContext _dbContext;

        public BlackjackGameRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(BlackjackGame game)
        {
            await _dbContext.BlackjackGames.AddAsync(game);
            // V této architektuře voláme SaveChanges rovnou v repozitáři
            await _dbContext.SaveChangesAsync();
        }

        public async Task<BlackjackGame> GetByIdAndUserIdAsync(Guid gameId, int userId)
        {
            // Zde je klíčové použití .Include() a .ThenInclude(),
            // abychom načetli celou hierarchii objektů (Hra -> Uživatel -> Peněženka).
            return await _dbContext.BlackjackGames
                .Include(g => g.User)
                .ThenInclude(u => u.Wallet)
                .FirstOrDefaultAsync(g => g.Id == gameId && g.UserId == userId);
        }

        public async Task UpdateAsync(BlackjackGame game)
        {
            _dbContext.BlackjackGames.Update(game);
            await _dbContext.SaveChangesAsync();
        }
    }
}