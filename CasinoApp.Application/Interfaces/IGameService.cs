using System.Threading.Tasks;
using CasinoApp.Domain.Models.Blackjack;

namespace CasinoApp.Application.Interfaces
{
    public interface IGameService
    {
        Task<GameResult> PlayDiceAsync(int userId, decimal stake, int guess);
        Task<GameResult> PlayRouletteAsync(int userId, decimal stake, string betType, string betValue);
        
        Task<GameResult> PlaySlotsAsync(int userId, decimal stake);
        
        Task<BlackjackGameState> StartBlackjackAsync(int userId, decimal stake);

        Task<BlackjackGameState> BlackjackHitAsync(Guid gameId, int userId);

        Task<BlackjackGameState> BlackjackStandAsync(Guid gameId, int userId);
    }

    public class GameResult
    {
        public bool IsWin { get; set; }
        public int RolledNumber { get; set; }
        public decimal WinAmount { get; set; }
        public decimal NewBalance { get; set; }
        public string Message { get; set; }
        public string[] Reels { get; set; }
    }
    
    public class BlackjackGameState
    {
        public Guid GameId { get; set; } 
        
        public List<Card> PlayerCards { get; set; } = new List<Card>();
        public List<Card> DealerCards { get; set; } = new List<Card>();
        
        public int PlayerScore { get; set; }
        public int DealerScore { get; set; }
        
        public string Status { get; set; }

        public string Message { get; set; } 
        public decimal NewBalance { get; set; } 
        public decimal WinAmount { get; set; } 
        public bool IsWin { get; set; } 
    }
}