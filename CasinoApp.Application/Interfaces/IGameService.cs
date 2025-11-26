using System.Threading.Tasks;

namespace CasinoApp.Application.Interfaces
{
    public interface IGameService
    {
        Task<GameResult> PlayDiceAsync(int userId, decimal stake, int guess);
    }

    public class GameResult
    {
        public bool IsWin { get; set; }
        public int RolledNumber { get; set; }
        public decimal WinAmount { get; set; }
        public decimal NewBalance { get; set; }
        public string Message { get; set; }
    }
}