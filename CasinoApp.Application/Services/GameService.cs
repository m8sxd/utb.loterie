using CasinoApp.Application.Interfaces;
using CasinoApp.Domain.Entities;
using CasinoApp.Domain.Models.Blackjack;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CasinoApp.Application.Services
{
    public class GameService : IGameService
    {
        private readonly ITransactionManager _transactionManager;
        private readonly IWalletRepository _walletRepository;
        private readonly IBlackjackGameRepository _blackjackRepository;
        private readonly Random _random = new Random();
        private readonly int[] _redNumbers = { 1, 3, 5, 7, 9, 12, 14, 16, 18, 19, 21, 23, 25, 27, 30, 32, 34, 36 };

        public GameService(ITransactionManager transactionManager, IWalletRepository walletRepository, IBlackjackGameRepository blackjackRepository)
        {
            _transactionManager = transactionManager;
            _walletRepository = walletRepository;
            _blackjackRepository = blackjackRepository;
        }

        public async Task<GameResult> PlayDiceAsync(int userId, decimal stake, int guess)
        {
            if (stake <= 0) throw new ArgumentException("Sázka musí být kladná.");
            if (guess < 1 || guess > 6) throw new ArgumentException("Tip musí být 1-6.");

            var result = new GameResult();

            await _transactionManager.ExecuteTransactionAsync(async () =>
            {
                var wallet = await _walletRepository.GetByUserIdAsync(userId);

                if (wallet == null || wallet.Balance < stake)
                    throw new InvalidOperationException("Nedostatek prostředků.");

                int rolled = _random.Next(1, 7);
                result.RolledNumber = rolled;
                bool isWin = (rolled == guess);
                result.IsWin = isWin;

                decimal winAmount = 0;

                if (isWin)
                {
                    winAmount = stake * 6;
                    wallet.Balance = wallet.Balance - stake + winAmount; 
                    result.Message = $"VÝHRA! Padla {rolled}. Vyhráváte {winAmount:C}.";
                }
                else
                {
                    wallet.Balance -= stake;
                    result.Message = $"Prohra. Padla {rolled}, tipoval jste {guess}.";
                }

                result.WinAmount = winAmount;
                result.NewBalance = wallet.Balance;

                var transaction = new Transaction
                {
                    Id = Guid.NewGuid(),
                    WalletId = wallet.Id,
                    Type = isWin ? "GameWin" : "GameLoss",
                    Amount = isWin ? (winAmount - stake) : -stake,
                    BalanceAfter = wallet.Balance,
                    Note = $"Dice: Tip {guess}, Hod {rolled}",
                    CreatedAt = DateTime.UtcNow
                };

                await _walletRepository.AddTransactionAsync(transaction);
                await _walletRepository.UpdateAsync(wallet);
            });

            return result;
        }

        public async Task<GameResult> PlayRouletteAsync(int userId, decimal stake, string betType, string betValue)
        {
            if (stake <= 0) throw new ArgumentException("Sázka musí být kladná.");

            var result = new GameResult();

            await _transactionManager.ExecuteTransactionAsync(async () =>
            {
                var wallet = await _walletRepository.GetByUserIdAsync(userId);

                if (wallet == null || wallet.Balance < stake)
                    throw new InvalidOperationException("Nedostatek prostředků.");

 
                int rolled = _random.Next(0, 37);
                result.RolledNumber = rolled;

                bool isWin = false;
                decimal multiplier = 0;

                if (betType == "Number")
                {
                    if (int.TryParse(betValue, out int numVal) && numVal == rolled)
                    {
                        isWin = true; multiplier = 35;
                    }
                }
                else if (betType == "Color" && rolled != 0)
                {
                    bool isRed = _redNumbers.Contains(rolled);
                    if ((betValue == "Red" && isRed) || (betValue == "Black" && !isRed))
                    {
                        isWin = true; multiplier = 1;
                    }
                }
                else if (betType == "Parity" && rolled != 0)
                {
                    bool isEven = (rolled % 2 == 0);
                    if ((betValue == "Even" && isEven) || (betValue == "Odd" && !isEven))
                    {
                        isWin = true; multiplier = 1;
                    }
                }

                result.IsWin = isWin;
                decimal winAmount = 0;

                if (isWin)
                {
                    winAmount = stake + (stake * multiplier);
                    wallet.Balance = wallet.Balance - stake + winAmount;
                    result.Message = $"VÝHRA! Padla {rolled}.";
                }
                else
                {
                    wallet.Balance -= stake;
                    result.Message = $"Prohra. Padla {rolled}.";
                }

                result.WinAmount = winAmount;
                result.NewBalance = wallet.Balance;

                var transaction = new Transaction
                {
                    Id = Guid.NewGuid(),
                    WalletId = wallet.Id,
                    Type = isWin ? "RouletteWin" : "RouletteLoss",
                    Amount = isWin ? (winAmount - stake) : -stake,
                    BalanceAfter = wallet.Balance,
                    Note = $"Roulette: {betType} {betValue}, Hod {rolled}",
                    CreatedAt = DateTime.UtcNow
                };

                await _walletRepository.AddTransactionAsync(transaction);
                await _walletRepository.UpdateAsync(wallet);
            });

            return result;
        }

        public async Task<GameResult> PlaySlotsAsync(int userId, decimal stake)
        {
            if (stake <= 0) throw new ArgumentException("Sázka musí být kladná.");

            var result = new GameResult();

            await _transactionManager.ExecuteTransactionAsync(async () =>
            {
                var wallet = await _walletRepository.GetByUserIdAsync(userId);

                if (wallet == null || wallet.Balance < stake)
                    throw new InvalidOperationException("Nedostatek prostředků.");

                var paytable = new Dictionary<string, decimal>
                {
                    { "🍒", 5m }, { "🍋", 10m }, { "🍇", 20m }, { "🔔", 50m }, { "💎", 100m }, { "💩", 0m }
                };
                string[] availableSymbols = paytable.Keys.ToArray();

                string[] reels = new string[3];
                for (int i = 0; i < 3; i++)
                {
                    reels[i] = availableSymbols[_random.Next(0, availableSymbols.Length)];
                }
                result.Reels = reels; 

                bool isWin = false;
                decimal multiplier = 0;

                if (reels[0] == reels[1] && reels[1] == reels[2])
                {
                    string winningSymbol = reels[0];
                    if (paytable.ContainsKey(winningSymbol))
                    {
                        multiplier = paytable[winningSymbol];
                        if (multiplier > 0) isWin = true;
                    }
                }

                result.IsWin = isWin;
                decimal winAmount = 0;

                if (isWin)
                {
                    winAmount = stake * multiplier;
                    wallet.Balance = wallet.Balance - stake + winAmount; 
                    result.Message = $"VÝHRA! Tři {reels[0]}.";
                }
                else
                {
                    wallet.Balance -= stake;
                    result.Message = "Prohra. Zkus to znovu.";
                }

                await _walletRepository.UpdateAsync(wallet);

                result.WinAmount = winAmount;
                result.NewBalance = wallet.Balance;

                var transaction = new Transaction
                {
                    Id = Guid.NewGuid(),
                    WalletId = wallet.Id,
                    Type = isWin ? "SlotsWin" : "SlotsLoss",
                    Amount = isWin ? (winAmount - stake) : -stake,
                    BalanceAfter = wallet.Balance,
                    Note = $"Slots: {string.Join(" ", reels)}",
                    CreatedAt = DateTime.UtcNow
                };

                await _walletRepository.AddTransactionAsync(transaction);
            });

            return result;
        }

        public async Task<BlackjackGameState> StartBlackjackAsync(int userId, decimal stake)
        {
            if (stake <= 0) throw new ArgumentException("Sázka musí být kladná.");
            var gameState = new BlackjackGameState();

            await _transactionManager.ExecuteTransactionAsync(async () =>
            {
                var wallet = await _walletRepository.GetByUserIdAsync(userId);

                if (wallet == null || wallet.Balance < stake)
                    throw new InvalidOperationException("Nedostatek prostředků.");


                wallet.Balance -= stake;
                await _walletRepository.UpdateAsync(wallet);
                gameState.NewBalance = wallet.Balance;

                var deck = new Deck(); deck.Shuffle(); 
                var playerHand = new Hand();
                var dealerHand = new Hand();

                playerHand.AddCard(deck.DrawCard());
                dealerHand.AddCard(deck.DrawCard());
                playerHand.AddCard(deck.DrawCard());

                var hiddenDealerCard = deck.DrawCard();
                hiddenDealerCard.IsFaceUp = false;
                dealerHand.AddCard(hiddenDealerCard);

                var gameId = Guid.NewGuid();
                var blackjackGame = new BlackjackGame
                {
                    Id = gameId,
                    UserId = userId,
                    Stake = stake,
                    Status = "PlayerTurn",
                    Deck = SerializeDeck(deck),
                    PlayerHand = SerializeHand(playerHand.Cards),
                    DealerHand = SerializeHand(dealerHand.Cards),
                    StartTime = DateTime.UtcNow
                };

                if (playerHand.IsBlackjack)
                {
                    blackjackGame.Status = "Finished";
                    dealerHand.Cards[1].IsFaceUp = true;

                    if (dealerHand.IsBlackjack)
                    {
                        wallet.Balance += stake;
                        gameState.WinAmount = stake;
                        blackjackGame.ResultMessage = "Push (Remíza)";
                        gameState.Message = "Remíza.";
                    }
                    else
                    {
                        decimal win = stake * 2.5m; 
                        wallet.Balance += win;
                        gameState.WinAmount = win;
                        gameState.IsWin = true;
                        blackjackGame.ResultMessage = "Blackjack!";
                        gameState.Message = "BLACKJACK!";
                    }

                    blackjackGame.EndTime = DateTime.UtcNow;
                    blackjackGame.DealerHand = SerializeHand(dealerHand.Cards);
                    await _walletRepository.UpdateAsync(wallet);
                }
                else
                {
                    gameState.Message = "Hit nebo Stand?";
                }

                await _blackjackRepository.AddAsync(blackjackGame);

                gameState.GameId = gameId;
                gameState.PlayerCards = playerHand.Cards;
                gameState.DealerCards = dealerHand.Cards;
                gameState.PlayerScore = playerHand.GetScore();
                gameState.DealerScore = dealerHand.GetScore();
                gameState.Status = blackjackGame.Status;
                gameState.NewBalance = wallet.Balance;
            });
            return gameState;
        }

        public async Task<BlackjackGameState> BlackjackHitAsync(Guid gameId, int userId)
        {
            var gameState = new BlackjackGameState();
            await _transactionManager.ExecuteTransactionAsync(async () =>
            {
                var game = await _blackjackRepository.GetByIdAndUserIdAsync(gameId, userId);
                if (game == null || game.Status != "PlayerTurn") throw new InvalidOperationException("Nelze hrát.");

                var deck = DeserializeDeck(game.Deck);
                var playerHand = new Hand { Cards = DeserializeHand(game.PlayerHand) };
                var dealerHand = new Hand { Cards = DeserializeHand(game.DealerHand) };

                playerHand.AddCard(deck.DrawCard());
                if (playerHand.IsBusted)
                {
                    game.Status = "Finished";
                    game.EndTime = DateTime.UtcNow;
                    game.ResultMessage = "Bust (Přesáhl)";
                    gameState.Message = "Bust! Prohra.";
                }
                else
                {
                    gameState.Message = "Hit nebo Stand?";
                }

                game.PlayerHand = SerializeHand(playerHand.Cards);
                game.Deck = SerializeDeck(deck);

                await _blackjackRepository.UpdateAsync(game);

                gameState.GameId = game.Id;
                gameState.PlayerCards = playerHand.Cards;
                gameState.DealerCards = dealerHand.Cards;
                gameState.PlayerScore = playerHand.GetScore();
                gameState.DealerScore = dealerHand.GetScore();
                gameState.Status = game.Status;
                gameState.NewBalance = game.User.Wallet.Balance;
            });
            return gameState;
        }

        public async Task<BlackjackGameState> BlackjackStandAsync(Guid gameId, int userId)
        {
            var gameState = new BlackjackGameState();

            await _transactionManager.ExecuteTransactionAsync(async () =>
            {
                var game = await _blackjackRepository.GetByIdAndUserIdAsync(gameId, userId);
                if (game == null || game.Status != "PlayerTurn") throw new InvalidOperationException("Hru nelze hrát.");

                game.Status = "DealerTurn";
                var deck = DeserializeDeck(game.Deck);
                var playerHand = new Hand { Cards = DeserializeHand(game.PlayerHand) };
                var dealerHand = new Hand { Cards = DeserializeHand(game.DealerHand) };

                dealerHand.Cards[1].IsFaceUp = true;
                while (dealerHand.GetScore() < 17) { dealerHand.AddCard(deck.DrawCard()); }

                int pScore = playerHand.GetScore();
                int dScore = dealerHand.GetScore();
                decimal winAmount = 0;

                if (dealerHand.IsBusted) { winAmount = game.Stake * 2; gameState.IsWin = true; game.ResultMessage = "Dealer Bust"; gameState.Message = "Dealer přesáhl! Výhra."; }
                else if (dScore > pScore) { game.ResultMessage = $"Prohra ({pScore} vs {dScore})"; gameState.Message = $"Dealer má {dScore}. Prohra."; }
                else if (dScore < pScore) { winAmount = game.Stake * 2; gameState.IsWin = true; game.ResultMessage = $"Výhra ({pScore} vs {dScore})"; gameState.Message = $"Máš {pScore}! Výhra."; }
                else { winAmount = game.Stake; game.ResultMessage = "Push"; gameState.Message = "Remíza."; }

                Wallet walletForDto = game.User.Wallet;

                if (winAmount > 0)
                {
                    var freshWallet = await _walletRepository.GetByUserIdAsync(userId);
                    freshWallet.Balance += winAmount;

                    var transaction = new Transaction
                    {
                        Id = Guid.NewGuid(),
                        Type = gameState.IsWin ? "BlackjackWin" : "BlackjackPush",
                        Amount = winAmount - game.Stake,
                        BalanceAfter = freshWallet.Balance,
                        Note = game.ResultMessage,
                        CreatedAt = DateTime.UtcNow
                    };

                    await _walletRepository.AddTransactionAsync(transaction);
                    await _walletRepository.UpdateAsync(freshWallet);
                    walletForDto = freshWallet;
                }

                game.Status = "Finished";
                game.EndTime = DateTime.UtcNow;
                game.DealerHand = SerializeHand(dealerHand.Cards);
                game.Deck = SerializeDeck(deck);
                await _blackjackRepository.UpdateAsync(game);

                gameState.GameId = game.Id;
                gameState.PlayerCards = playerHand.Cards;
                gameState.DealerCards = dealerHand.Cards;
                gameState.PlayerScore = pScore;
                gameState.DealerScore = dScore;
                gameState.Status = game.Status;
                gameState.NewBalance = walletForDto.Balance;
            });

            return gameState;
        }

        private string SerializeHand(List<Card> cards) => string.Join(",", cards.Select(c => $"{c.Suit.ToString()[0]}-{c.Rank.ToString()}:{(c.IsFaceUp ? "T" : "F")}"));

        private List<Card> DeserializeHand(string handData)
        {
            var cards = new List<Card>();
            if (string.IsNullOrEmpty(handData)) return cards;
            foreach (var cardStr in handData.Split(','))
            {
                var parts = cardStr.Split(new[] { '-', ':' });
                if (parts.Length >= 3)
                {
                    var suit = GetSuitFromChar(parts[0]);
                    var rank = (Rank)Enum.Parse(typeof(Rank), parts[1]);
                    cards.Add(new Card(suit, rank) { IsFaceUp = parts[2] == "T" });
                }
            }
            return cards;
        }

        private string SerializeDeck(Deck deck) { var f = typeof(Deck).GetField("_cards", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance); return SerializeHand((List<Card>)f.GetValue(deck)); }
        private Deck DeserializeDeck(string d) { var deck = new Deck(); var f = typeof(Deck).GetField("_cards", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance); f.SetValue(deck, DeserializeHand(d)); return deck; }

        private Suit GetSuitFromChar(string c) => c switch { "H" => Suit.Hearts, "D" => Suit.Diamonds, "C" => Suit.Clubs, "S" => Suit.Spades, _ => Suit.Hearts };
    }
}