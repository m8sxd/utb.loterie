using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace CasinoApp.Infrastructure.Database.Seeding
{
    public class UserRoleSeeding
    {
        public IdentityUserRole<int> GetAdminAssignment()
        {
            return new IdentityUserRole<int>
            {
                UserId = 1,
                RoleId = 1 
            };
        }
        
        public IdentityUserRole<int> GetPlayerAssignment()
        {
            return new IdentityUserRole<int>
            {
                UserId = 2,
                RoleId = 2 
            };
        }

        public List<IdentityUserRole<int>> GetAllAssignments()
        {
            return new List<IdentityUserRole<int>> { GetAdminAssignment(), GetPlayerAssignment() };
        }
    }
}