using CasinoApp.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace CasinoApp.Infrastructure.Database.Seeding
{
    public class UserSeeding
    {
        private readonly PasswordHasher<User> _passwordHasher = new PasswordHasher<User>();

        public User GetAdminUser()
        {
            var user = new User
            {
                Id = 1,
                UserName = "admin",
                NormalizedUserName = "ADMIN",
                Email = "admin@casino.cz",
                NormalizedEmail = "ADMIN@CASINO.CZ",
                EmailConfirmed = true,
                SecurityStamp = "D9F5C3B1-A2E4-4F6D-8C0B-1E3F5A7D9C1B"
            };

            // Zde dynamicky vygenerujeme hash pro heslo "admin123"
            user.PasswordHash = _passwordHasher.HashPassword(user, "admin123");

            return user;
        }

        public User GetPlayerUser()
        {
            var user = new User
            {
                Id = 2,
                UserName = "hrac",
                NormalizedUserName = "HRAC",
                Email = "hrac@casino.cz",
                NormalizedEmail = "HRAC@CASINO.CZ",
                EmailConfirmed = true,
                SecurityStamp = "E1F2C3B4-A5E6-4F7D-8C9B-0E1F2A3D4C5B"
            };

            // Zde dynamicky vygenerujeme hash pro heslo "hrac123"
            user.PasswordHash = _passwordHasher.HashPassword(user, "hrac123");

            return user;
        }

        public List<User> GetAllUsers()
        {
            return new List<User> { GetAdminUser(), GetPlayerUser() };
        }
    }
}