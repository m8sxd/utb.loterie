using CasinoApp.Application.Interfaces;
using CasinoApp.Domain.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CasinoApp.Application.Services
{
    public class GameService : IGameService
    {
        private readonly ITransactionManager _transactionManager;
        private readonly IWalletRepository _walletRepository;
        private readonly Random _random = new Random();
        private readonly int[] _redNumbers = { 1, 3, 5, 7, 9, 12, 14, 16, 18, 19, 21, 23, 25, 27, 30, 32, 34, 36 };

        public GameService(ITransactionManager transactionManager, IWalletRepository walletRepository)
        {
            _transactionManager = transactionManager;
            _walletRepository = walletRepository;
        }

        public async Task<GameResult> PlayDiceAsync(int userId, decimal stake, int guess)
        {
            var result = new GameResult();

            if (stake <= 0) { result.Message = "Sázka musí být kladná."; return result; }
            if (guess < 1 || guess > 6) { result.Message = "Tip 1-6."; return result; }

            await _transactionManager.ExecuteTransactionAsync(async () =>
            {
                var wallet = await _walletRepository.GetByUserIdAsync(userId);

                if (wallet == null || wallet.Balance < stake)
                {
                    result.Message = "Nedostatek prostředků.";
                    return;
                }

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
            var result = new GameResult();

            if (stake <= 0) { result.Message = "Sázka musí být kladná."; return result; }

            await _transactionManager.ExecuteTransactionAsync(async () =>
            {
                var wallet = await _walletRepository.GetByUserIdAsync(userId);

                if (wallet == null || wallet.Balance < stake)
                {
                    result.Message = "Nedostatek prostředků.";
                    return;
                }

                int rolled = _random.Next(0, 37);
                result.RolledNumber = rolled;

                bool isWin = false;
                decimal multiplier = 0;

                if (betType == "Number")
                {
                    if (int.TryParse(betValue, out int numVal) && numVal == rolled)
                    {
                        isWin = true;
                        multiplier = 35;
                    }
                }
                else if (betType == "Color")
                {
                    if (rolled != 0)
                    {
                        bool isRed = _redNumbers.Contains(rolled);
                        if ((betValue == "Red" && isRed) || (betValue == "Black" && !isRed))
                        {
                            isWin = true;
                            multiplier = 1;
                        }
                    }
                }
                else if (betType == "Parity")
                {
                    if (rolled != 0)
                    {
                        bool isEven = (rolled % 2 == 0);
                        if ((betValue == "Even" && isEven) || (betValue == "Odd" && !isEven))
                        {
                            isWin = true;
                            multiplier = 1;
                        }
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
            var result = new GameResult();

            if (stake <= 0) { result.Message = "Sázka musí být kladná."; return result; }

            await _transactionManager.ExecuteTransactionAsync(async () =>
            {
                var wallet = await _walletRepository.GetByUserIdAsync(userId);

                if (wallet == null || wallet.Balance < stake)
                {
                    result.Message = "Nedostatek prostředků.";
                    return;
                }

                // 1. Definice symbolů a výplat
                var paytable = new Dictionary<string, decimal>
                {
                    { "🍒", 5m },   // Třešně - 5x
                    { "🍋", 10m },  // Citron - 10x
                    { "🍇", 20m },  // Hrozny - 20x
                    { "🔔", 50m },  // Zvonky - 50x
                    { "💎", 100m }, // Diamant - 100x
                    { "💩", 0m }    // Smůla - 0x
                };
                string[] availableSymbols = paytable.Keys.ToArray();

                // 2. Generování 3 symbolů (válců)
                string[] reels = new string[3];
                for (int i = 0; i < 3; i++)
                {
                    reels[i] = availableSymbols[_random.Next(0, availableSymbols.Length)];
                }
                result.Reels = reels; // Uložíme pro frontend

                // 3. Vyhodnocení výhry (3 stejné)
                bool isWin = false;
                decimal multiplier = 0;

                if (reels[0] == reels[1] && reels[1] == reels[2])
                {
                    string winningSymbol = reels[0];
                    multiplier = paytable[winningSymbol];
                    if (multiplier > 0)
                    {
                        isWin = true;
                    }
                }

                result.IsWin = isWin;
                decimal winAmount = 0;

                // 4. Aktualizace zůstatku
                if (isWin)
                {
                    // Výhra: Vklad se vrací + zisk (stake * multiplier)
                    winAmount = stake + (stake * multiplier);
                    wallet.Balance = wallet.Balance - stake + winAmount;
                    result.Message = $"VÝHRA! Tři {reels[0]}.";
                }
                else
                {
                    // Prohra: Jen se odečte sázka
                    wallet.Balance -= stake;
                    result.Message = "Prohra. Zkus to znovu.";
                }

                result.WinAmount = winAmount;
                result.NewBalance = wallet.Balance;

                // 5. Záznam transakce
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
                await _walletRepository.UpdateAsync(wallet);
            });

            return result;
        }
    }
}