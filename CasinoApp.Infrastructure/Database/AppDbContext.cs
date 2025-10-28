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

            modelBuilder.Entity<Wallet>()
                .Property(w => w.ConcurrencyToken)
                .IsRowVersion();

            modelBuilder.Entity<Wallet>()
                .HasIndex(w => w.UserId)
                .IsUnique();

            modelBuilder.Entity<Transaction>()
                .HasIndex(t => t.WalletId);

            modelBuilder.Entity<Transaction>()
                .HasIndex(t => t.CreatedAt);

            modelBuilder.Entity<Bet>()
                .HasMany(b => b.Selections)
                .WithOne(s => s.Bet)
                .HasForeignKey(s => s.BetId);

            modelBuilder.Entity<Bet>()
                .HasOne(b => b.Wallet)
                .WithMany()
                .HasForeignKey(b => b.WalletId);
        }
    }
}