using CasinoApp.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace CasinoApp.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(int userId);
        Task<User?> GetByUsernameAsync(string username);
        Task AddAsync(User user);

    }
}