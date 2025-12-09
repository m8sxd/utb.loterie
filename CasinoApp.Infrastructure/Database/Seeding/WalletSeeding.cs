using CasinoApp.Domain.Entities;

namespace CasinoApp.Infrastructure.Database.Seeding
{
    public class WalletSeeding
    {
        public Wallet GetAdminWallet()
        {
            return new Wallet
            {
                Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), 
                UserId = 1, 
                Currency = "CZK",
                Balance = 1000000

            };
        }

        public Wallet GetPlayerWallet()
        {
            return new Wallet
            {
                Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                UserId = 2, 
                Currency = "CZK",
                Balance = 50000
            };
        }

        public List<Wallet> GetAllWallets()
        {
            return new List<Wallet> { GetAdminWallet(), GetPlayerWallet() };
        }
    }
}