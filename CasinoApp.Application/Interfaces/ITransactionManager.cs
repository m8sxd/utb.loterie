using System;
using System.Threading.Tasks;

namespace CasinoApp.Application.Interfaces
{
    public interface ITransactionManager
    {
        Task ExecuteTransactionAsync(Func<Task> operation);
    }
}