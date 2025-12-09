using CasinoApp.Domain.Entities;

namespace CasinoApp.Infrastructure.Database.Seeding
{
    public class UserSeeding
    {
        // admin123 -> AQAAAAEAACcQAAAAEHfQ6Y4Z4w6Q4Z4w6Q4Z4w6Q4Z4w6Q4Z4w6Q4Z4w6Q4Z4w6Q4Z4w6Q==
        // hrac123  -> AQAAAAEAACcQAAAAELfQ6Y4Z4w6Q4Z4w6Q4Z4w6Q4Z4w6Q4Z4w6Q4Z4w6Q4Z4w6Q4Z4w6Q==

        public User GetAdminUser()
        {
            return new User
            {  // Heslo je admin123
                Id = 1,
                UserName = "admin",
                NormalizedUserName = "ADMIN",
                Email = "admin@casino.cz",
                NormalizedEmail = "ADMIN@CASINO.CZ",
                EmailConfirmed = true,
                PasswordHash = "AQAAAAIAAYagAAAAEOl9icASrR1GYhtpadevd7l4QaF9N8flzBw7MEezhZuxlPxOMJX6l7s3ovYxbgh/Jg==",
                SecurityStamp = "D9F5C3B1-A2E4-4F6D-8C0B-1E3F5A7D9C1B" // Pevný GUID pro konzistenci
            };
        }

        public User GetPlayerUser()
        { //Heslo je hrac123
            return new User
            {
                Id = 2,
                UserName = "hrac",
                NormalizedUserName = "HRAC",
                Email = "hrac@casino.cz",
                NormalizedEmail = "HRAC@CASINO.CZ",
                EmailConfirmed = true,
                PasswordHash = "AQAAAAIAAYagAAAAEM0RxbGBS4qk/jUIHnMLlf42ISI6XhE4d0K56o2vXeUoGi+yD7npVIOG/e1xuAq4aw==",
                SecurityStamp = "E1F2C3B4-A5E6-4F7D-8C9B-0E1F2A3D4C5B"
            };
        }


        public List<User> GetAllUsers()
        {
            return new List<User> { GetAdminUser(), GetPlayerUser() };
        }
    }
}