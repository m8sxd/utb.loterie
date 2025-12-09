using CasinoApp.Domain.Entities;
using System.Collections.Generic;

namespace CasinoApp.Infrastructure.Database.Seeding
{
    public class RoleSeeding
    {
        public Role GetAdminRole()
        {
            return new Role
            {
                Id = 1,
                Name = "Admin",
                NormalizedName = "ADMIN",
                ConcurrencyStamp = "7217E312-199C-4403-A66E-E9A62E030652"
            };
        }

        public Role GetUserRole()
        {
            return new Role
            {
                Id = 2,
                Name = "User",
                NormalizedName = "USER",
                ConcurrencyStamp = "52337986-981C-4B11-8337-82166C274057"
            };
        }

        public List<Role> GetAllRoles()
        {
            return new List<Role> { GetAdminRole(), GetUserRole() };
        }
    }
}