using CasinoApp.Application.Interfaces;
using CasinoApp.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace utb.loterie.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IWalletRepository _walletRepository; 

        public AdminController(UserManager<User> userManager, IWalletRepository walletRepository)
        {
            _userManager = userManager;
            _walletRepository = walletRepository;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            var model = new List<UserAdminViewModel>();

            foreach (var u in users)
            {
                var wallet = await _walletRepository.GetByUserIdAsync(u.Id);

                model.Add(new UserAdminViewModel
                {
                    Id = u.Id,
                    Username = u.UserName,
                    Email = u.Email,
                    IsBanned = u.LockoutEnd.HasValue && u.LockoutEnd.Value > DateTimeOffset.UtcNow,
                    Balance = wallet != null ? wallet.Balance : 0 
                });
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleBan(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return NotFound();

            if (User.Identity.Name == user.UserName)
            {
                TempData["Error"] = "Nemùžeš zabanovat sám sebe!";
                return RedirectToAction("Index");
            }

            if (user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTimeOffset.UtcNow)
            {
                await _userManager.SetLockoutEndDateAsync(user, null);
                TempData["Success"] = $"Uživatel {user.UserName} byl odbanován.";
            }
            else
            {
                await _userManager.SetLockoutEnabledAsync(user, true);
                await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddYears(100));
                TempData["Success"] = $"Uživatel {user.UserName} byl zabanován.";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> ChangeBalance(int userId, decimal amount)
        {
            var wallet = await _walletRepository.GetByUserIdAsync(userId);
            if (wallet == null)
            {
                TempData["Error"] = "Uživatel nemá penìženku.";
                return RedirectToAction("Index");
            }

            wallet.Balance += amount;

            if (wallet.Balance < 0) wallet.Balance = 0;

            await _walletRepository.UpdateAsync(wallet);

            TempData["Success"] = $"Zùstatek uživatele byl upraven o {amount} Kè.";
            return RedirectToAction("Index");
        }
    }

    public class UserAdminViewModel
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public bool IsBanned { get; set; }
        public decimal Balance { get; set; } 
    }
}