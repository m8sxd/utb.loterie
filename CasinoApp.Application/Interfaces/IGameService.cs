using System.Threading.Tasks;

namespace CasinoApp.Application.Interfaces
{
    public interface IGameService
    {
        Task<GameResult> PlayDiceAsync(int userId, decimal stake, int guess);
        Task<GameResult> PlayRouletteAsync(int userId, decimal stake, string betType, string betValue);
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