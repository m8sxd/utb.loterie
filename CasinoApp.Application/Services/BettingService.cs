using CasinoApp.Application.Interfaces;
using CasinoApp.Domain.Entities;
// ODSTRANĚNO: using CasinoApp.Infrastructure.Database;
using Microsoft.EntityFrameworkCore; // Potřebné pro DbUpdateConcurrencyException

namespace CasinoApp.Application.Services
{
    public class BettingService : IBettingService
    {
        private readonly ITransactionManager _transactionManager;
        private readonly IWalletRepository _walletRepository;
        // Předpokládáme existenci IBetRepository, kterou implementujeme později.
        // private readonly IBetRepository _betRepository; 

        // 👈 Nyní závisí pouze na ROZHRANÍCH z Application
        public BettingService(ITransactionManager transactionManager, IWalletRepository walletRepository)
        {
            _transactionManager = transactionManager;
            _walletRepository = walletRepository;
        }

        public async Task<(Bet? Bet, string Error)> PlaceBetAsync(Guid userId, decimal stake, List<BetSelection> selections)
        {
            if (stake <= 0)
            {
                return (null, "Sázka musí být kladná.");
            }

            // Výsledek musí být definován mimo lambda výraz
            (Bet? Bet, string Error) result = (null, "Transakce se nezdařila, zkuste to prosím znovu.");

            // Používáme rozhraní pro řízení transakce
            await _transactionManager.ExecuteTransactionAsync(async () =>
            {
                var wallet = await _walletRepository.GetByUserIdAsync(userId);
                if (wallet == null || wallet.Balance < stake)
                {
                    result = (null, "Nedostatečný zůstatek.");
                    return;
                }

                // 1. Zámek peněz (přesun do Reserved)
                wallet.UpdateBalance(stake, isReserved: true);
                await _walletRepository.UpdateAsync(wallet);
                // Poznámka: SaveChangesAsync zde VOLAT NEBUDEME, proběhne až na konci transakce v TransactionManageru

                // 2. Vytvoření Bet a Selections (zde by se ideálně volalo IBetRepository.Add)
                var newBet = new Bet
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    WalletId = wallet.Id,
                    Stake = stake,
                    PotentialPayout = CalculatePotentialPayout(stake, selections),
                    Selections = selections,
                    Status = "Pending"
                };
                // _betRepository.Add(newBet);

                // --- PRO JEDNODUCHOST PŘEDPOKLÁDÁME ZÁPIS PŘES REPOZITÁŘ ---
                // Musíme zajistit, aby Repozitáře ukládaly do kontextu, 
                // ale nevolaly SaveChangesAsync, pokud běží transakce.
                // Toto je největší komplikace UoW/Transaction managementu v EF Core.

                // Místo toho, aby Repozitáře volaly SaveChangesAsync, voláme ho jen jednou z TransactionManageru.

                // **Při reálné implementaci je nutné přepsat všechny Repozitáře, 
                // aby SaveChangesAsync volaly pouze na konci transakce, nebo je UoW/Manager volal sám.**

                // Prozatím předpokládejme, že Repozitáře přidávají data do _context, ale neukládají.

                // 3. Vytvoření Transaction (Audit)
                var betStakeTransaction = new Transaction
                {
                    WalletId = wallet.Id,
                    Type = "BetStake",
                    Amount = -stake,
                    BalanceAfter = wallet.Balance + wallet.Reserved,
                    ReferenceId = newBet.Id,
                    Note = $"Sázka ID: {newBet.Id}"
                };
                // _transactionRepository.Add(betStakeTransaction);


                result = (newBet, string.Empty);

                // Poznámka: Rollback/Commit se řídí v ITransactionManager.ExecuteTransactionAsync!
            });

            return result;
        }

        private decimal CalculatePotentialPayout(decimal stake, List<BetSelection> selections)
        {
            if (!selections.Any()) return 0;

            decimal totalOdds = selections.Aggregate(1.0m, (current, selection) => current * selection.DecimalOdds);

            return Math.Round(stake * totalOdds, 2);
        }

        public Task<bool> SettleBetAsync(Guid betId, string finalOutcome)
        {
            throw new NotImplementedException();
        }
    }
}