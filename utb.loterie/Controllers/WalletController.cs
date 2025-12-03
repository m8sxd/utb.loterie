using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using CasinoApp.Application.Interfaces;

namespace CasinoApp.Controllers
{
    [Authorize] 
    public class WalletController : Controller
    {
        private readonly IWalletRepository _walletRepository;

        public WalletController(IWalletRepository walletRepository)
        {
            _walletRepository = walletRepository;
        }

        [HttpPost]
        public async Task<IActionResult> Deposit(decimal amount)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var wallet = await _walletRepository.GetByUserIdAsync(userId);
            if (wallet != null)
            {
                wallet.Balance += amount;

                await _walletRepository.UpdateAsync(wallet);
            }

            return Redirect(Request.Headers["Referer"].ToString());
        }
    }
}