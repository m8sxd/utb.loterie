using CasinoApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using CasinoApp.Infrastructure.Database.Seeding;
using Microsoft.AspNetCore.Identity;
namespace CasinoApp.Infrastructure.Database
{
    public class AppDbContext : IdentityDbContext<User, Role, int>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Game> Games { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Bet> Bets { get; set; }
        public DbSet<BetSelection> BetSelections { get; set; }
        public DbSet<BlackjackGame> BlackjackGames { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(b =>
            {
                b.Property(u => u.UserName).HasMaxLength(128);
                b.Property(u => u.NormalizedUserName).HasMaxLength(128);
                b.Property(u => u.Email).HasMaxLength(128);
                b.Property(u => u.NormalizedEmail).HasMaxLength(128);
            });

            modelBuilder.Entity<Role>(b =>
            {
                b.Property(r => r.Name).HasMaxLength(128);
                b.Property(r => r.NormalizedName).HasMaxLength(128);
            });

            modelBuilder.Entity<Microsoft.AspNetCore.Identity.IdentityUserLogin<int>>(b =>
            {
                b.Property(l => l.LoginProvider).HasMaxLength(128);
                b.Property(l => l.ProviderKey).HasMaxLength(128);
            });
            modelBuilder.Entity<Microsoft.AspNetCore.Identity.IdentityUserToken<int>>(b =>
            {
                b.Property(t => t.LoginProvider).HasMaxLength(128);
                b.Property(t => t.Name).HasMaxLength(128);
            });

            modelBuilder.Entity<User>(e =>
            {
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

            modelBuilder.Entity<BlackjackGame>(e =>
            {
                e.HasKey(b => b.Id);
                e.HasOne(b => b.User)
                    .WithMany()
                    .HasForeignKey(b => b.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
            
            var userSeeding = new UserSeeding();
            var walletSeeding = new WalletSeeding();
            var roleSeeding = new RoleSeeding();
            var userRoleSeeding = new UserRoleSeeding();
            
            modelBuilder.Entity<User>().HasData(userSeeding.GetAllUsers().ToArray());
            modelBuilder.Entity<Wallet>().HasData(walletSeeding.GetAllWallets().ToArray());
            modelBuilder.Entity<Role>().HasData(roleSeeding.GetAllRoles().ToArray());
            modelBuilder.Entity<IdentityUserRole<int>>().HasData(userRoleSeeding.GetAllAssignments().ToArray());
        }
    }
}