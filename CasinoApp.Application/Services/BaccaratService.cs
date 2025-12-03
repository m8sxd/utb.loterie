using CasinoApp.Application.Interfaces;
using CasinoApp.Domain.Models.Blackjack;

namespace CasinoApp.Application.Services
{
    public class BaccaratResult
    {
        public List<Card> PlayerCards { get; set; } = new();
        public List<Card> BankerCards { get; set; } = new();
        public int PlayerScore { get; set; }
        public int BankerScore { get; set; }
        public string Winner { get; set; } 
        public decimal WinAmount { get; set; }
        public decimal NewBalance { get; set; }
        public string Message { get; set; }
    }

    public class BaccaratService
    {
        private readonly IWalletRepository _walletRepository;
        private readonly Random _random = new Random();

        public BaccaratService(IWalletRepository walletRepository)
        {
            _walletRepository = walletRepository;
        }

        public async Task<BaccaratResult> PlayRoundAsync(int userId, decimal stake, string betType)
        {
            var wallet = await _walletRepository.GetByUserIdAsync(userId);
            if (wallet == null || wallet.Balance < stake) throw new Exception("Nedostatek peněz.");

            wallet.Balance -= stake;

            var playerCards = new List<Card> { DrawCard(), DrawCard() };
            var bankerCards = new List<Card> { DrawCard(), DrawCard() };

            int pScore = CalculateScore(playerCards);
            int bScore = CalculateScore(bankerCards);
            bool natural = pScore >= 8 || bScore >= 8;

            if (!natural)
            {

                Card? playerThirdCard = null;
                if (pScore <= 5)
                {
                    playerThirdCard = DrawCard();
                    playerCards.Add(playerThirdCard);
                }

                if (ShouldBankerDraw(bScore, playerThirdCard))
                {
                    bankerCards.Add(DrawCard());
                }
            }

            pScore = CalculateScore(playerCards);
            bScore = CalculateScore(bankerCards);

            string winner = "Tie";
            if (pScore > bScore) winner = "Player";
            else if (bScore > pScore) winner = "Banker";

            decimal winAmount = 0;
            if (winner == betType)
            {
                if (betType == "Player") winAmount = stake * 2m; 
                else if (betType == "Banker") winAmount = stake * 1.95m; 
                else if (betType == "Tie") winAmount = stake * 9m; 
            }
            else if (winner == "Tie" && betType != "Tie")
            {
                winAmount = stake; 
            }

            if (winAmount > 0)
            {
                wallet.Balance += winAmount;
            }
            await _walletRepository.UpdateAsync(wallet);

            return new BaccaratResult
            {
                PlayerCards = playerCards,
                BankerCards = bankerCards,
                PlayerScore = pScore,
                BankerScore = bScore,
                Winner = winner,
                WinAmount = winAmount,
                NewBalance = wallet.Balance,
                Message = winAmount > 0 ? $"Výhra! ({winner})" : $"Vítězí {winner}"
            };
        }

        private Card DrawCard()
        {
            var suits = Enum.GetValues(typeof(Suit));
            var ranks = Enum.GetValues(typeof(Rank));
            return new Card((Suit)suits.GetValue(_random.Next(suits.Length)), (Rank)ranks.GetValue(_random.Next(ranks.Length)));
        }

        private int CalculateScore(List<Card> cards)
        {
            int sum = 0;
            foreach (var card in cards)
            {
                int val = (int)card.Rank;
                if (val >= 10) val = 0; 
                if (card.Rank == Rank.Ace) val = 1; 
                sum += val;
            }
            return sum % 10; 
        }

        private bool ShouldBankerDraw(int bankerScore, Card? playerThirdCard)
        {
            if (bankerScore >= 7) return false; 
            if (bankerScore <= 2) return true; 

            if (playerThirdCard == null) return bankerScore <= 5;

            int thirdVal = (int)playerThirdCard.Rank;
            if (thirdVal >= 10) thirdVal = 0;
            if (playerThirdCard.Rank == Rank.Ace) thirdVal = 1;

            if (bankerScore == 3 && thirdVal != 8) return true;
            if (bankerScore == 4 && (thirdVal >= 2 && thirdVal <= 7)) return true;
            if (bankerScore == 5 && (thirdVal >= 4 && thirdVal <= 7)) return true;
            if (bankerScore == 6 && (thirdVal == 6 || thirdVal == 7)) return true;

            return false;
        }
    }
}