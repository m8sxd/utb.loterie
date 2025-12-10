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

        public AdminController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();

            var model = users.Select(u => new UserAdminViewModel
            {
                Id = u.Id,
                Username = u.UserName,
                Email = u.Email,
                IsBanned = u.LockoutEnd.HasValue && u.LockoutEnd.Value > DateTimeOffset.UtcNow
            }).ToList();

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
    }

    public class UserAdminViewModel
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public bool IsBanned { get; set; }
    }
}