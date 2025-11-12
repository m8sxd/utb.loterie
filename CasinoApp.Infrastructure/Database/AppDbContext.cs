using CasinoApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CasinoApp.Infrastructure.Database
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Bet> Bets { get; set; }
        public DbSet<BetSelection> BetSelections { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(e =>
            {
                e.HasKey(u => u.Id);
                e.HasOne(u => u.Wallet)
                    .WithOne(w => w.User)
                    .HasForeignKey<Wallet>(w => w.UserId);

                e.HasMany(u => u.Bets)
                    .WithOne(b => b.User)
                    .HasForeignKey(b => b.UserId);
            });

            modelBuilder.Entity<Wallet>(e =>
            {
                e.HasKey(w => w.Id);
                e.Property(w => w.ConcurrencyToken).IsRowVersion(); 
                e.HasIndex(w => w.UserId).IsUnique(); 

                e.HasMany(w => w.Transactions)
                    .WithOne(t => t.Wallet)
                    .HasForeignKey(t => t.WalletId); 
            });

            modelBuilder.Entity<Bet>(e =>
            {
                e.HasKey(b => b.Id);
                e.HasMany(b => b.Selections)
                    .WithOne(s => s.Bet)
                    .HasForeignKey(s => s.BetId); 

                e.HasOne(b => b.Wallet)
                    .WithMany() 
                    .HasForeignKey(b => b.WalletId);
            });
        }
    }
}