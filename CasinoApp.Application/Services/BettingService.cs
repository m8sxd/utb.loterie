using CasinoApp.Application.Interfaces;
using CasinoApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
// PŘIDÁNO: Import pro Identity manažera
using Microsoft.AspNetCore.Identity;

namespace CasinoApp.Application.Services
{
    public class BettingService : IBettingService
    {
        private readonly ITransactionManager _transactionManager;
        private readonly IWalletRepository _walletRepository;

        private readonly UserManager<User> _userManager;
        private readonly IBetRepository _betRepository;

        public BettingService(
            ITransactionManager transactionManager,
            IWalletRepository walletRepository,

            UserManager<User> userManager,
            IBetRepository betRepository)
        {
            _transactionManager = transactionManager;
            _walletRepository = walletRepository;
            _userManager = userManager;
            _betRepository = betRepository;
        }

        public async Task<(Bet? Bet, string Error)> PlaceBetAsync(int userId, decimal stake, List<BetSelection> selections)
        {
            if (stake <= 0)
            {
                return (null, "Sázka musí být kladná.");
            }

            (Bet? Bet, string Error) result = (null, "Transakce se nezdařila, zkuste to prosím znovu.");

            try
            {
                await _transactionManager.ExecuteTransactionAsync(async () =>
                {

                    var user = await _userManager.FindByIdAsync(userId.ToString());


                    var wallet = await _walletRepository.GetByUserIdAsync(userId);

                    if (user == null || wallet == null)
                    {
                        result = (null, "Uživatel nebo peněženka nenalezena.");
                        return;
                    }

                    if (wallet.Balance < stake)
                    {
                        result = (null, "Nedostatečný zůstatek.");
                        return;
                    }

                    wallet.UpdateBalance(stake, isReserved: true);

                    var newBet = new Bet
                    {
                        Id = Guid.NewGuid(),
                        UserId = userId,
                        WalletId = wallet.Id,
                        Stake = stake,
                        PotentialPayout = CalculatePotentialPayout(stake, selections),
                        Selections = selections,
                        Status = "Pending",
                        PlacedAt = DateTime.UtcNow
                    };

                    var betStakeTransaction = new Transaction
                    {
                        Id = Guid.NewGuid(),
                        WalletId = wallet.Id,
                        Type = "BetStake",
                        Amount = -stake,
                        BalanceAfter = wallet.Balance,
                        ReferenceId = newBet.Id,
                        Note = $"Sázka ID: {newBet.Id}",
                        CreatedAt = DateTime.UtcNow
                    };


                    if (user.Bets == null) user.Bets = new List<Bet>();
                    user.Bets.Add(newBet);

                    wallet.Transactions.Add(betStakeTransaction);

            

                    result = (newBet, string.Empty);
                });
            }
            catch (DbUpdateConcurrencyException)
            {
                result = (null, "Chyba souběhu transakcí, zkuste to prosím znovu.");
            }
            catch (Exception ex)
            {
                result = (null, $"Došlo k chybě: {ex.Message}");
            }

            return result;
        }
        
        public async Task<(bool Success, string Error)> SettleBetAsync(Guid betId, bool isWinner)
        {
            (bool Success, string Error) result = (false, "Vyhodnocení se nezdařilo.");

            try
            {
                await _transactionManager.ExecuteTransactionAsync(async () =>
                {
                    var bet = await _betRepository.GetByIdWithWalletAsync(betId);

                    if (bet == null)
                    {
                        result = (false, "Sázka nenalezena.");
                        return;
                    }

                    var wallet = bet.Wallet;
                    if (wallet == null)
                    {
                        result = (false, "Peněženka propojená se sázkou nenalezena.");
                        return;
                    }

                    if (bet.Status != "Pending")
                    {
                        result = (false, "Sázka již byla vyhodnocena.");
                        return;
                    }

                    wallet.Reserved -= bet.Stake;
                    decimal winAmount = 0;

                    if (isWinner)
                    {
                        winAmount = bet.PotentialPayout;
                        wallet.Balance += winAmount;
                        bet.Status = "Won";
                    }
                    else
                    {
                        bet.Status = "Lost";
                    }

                    bet.SettledAt = DateTime.UtcNow;

                    var settleTransaction = new Transaction
                    {
                        Id = Guid.NewGuid(),
                        WalletId = wallet.Id,
                        Type = isWinner ? "BetWin" : "BetLoss",
                        Amount = winAmount,
                        BalanceAfter = wallet.Balance,
                        ReferenceId = bet.Id,
                        Note = $"Vyhodnocení sázky: {bet.Status}",
                        CreatedAt = DateTime.UtcNow
                    };

                    wallet.Transactions.Add(settleTransaction);

                    result = (true, string.Empty);
                });
            }
            catch (Exception ex)
            {
                result = (false, $"Došlo k chybě: {ex.Message}");
            }

            return result;
        }

        private decimal CalculatePotentialPayout(decimal stake, List<BetSelection> selections)
        {
            if (selections == null || !selections.Any()) return 0;
            decimal totalOdds = selections.Aggregate(1.0m, (current, selection) => current * selection.DecimalOdds);
            return Math.Round(stake * totalOdds, 2);
        }
    }
}