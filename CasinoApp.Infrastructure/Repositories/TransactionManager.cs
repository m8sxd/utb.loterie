using CasinoApp.Application.Interfaces;
using CasinoApp.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage; // PŘIDÁNO PRO IDbContextTransaction
using System;
using System.Threading.Tasks;

namespace CasinoApp.Infrastructure.Repositories
{
    public class TransactionManager : ITransactionManager
    {
        private readonly AppDbContext _context;

        public TransactionManager(AppDbContext context)
        {
            _context = context;
        }

        public async Task ExecuteTransactionAsync(Func<Task> operation)
        {
            var strategy = _context.Database.CreateExecutionStrategy();

            await strategy.ExecuteAsync(async () =>
            {
                // Zde se používá IDbContextTransaction (implicitně v BeginTransactionAsync)
                // Ale pro kompilátor je potřeba výše uvedený using.
                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        await operation();

                        await _context.SaveChangesAsync();

                        await transaction.CommitAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                    catch (Exception)
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                }
            });
        }
    }
}