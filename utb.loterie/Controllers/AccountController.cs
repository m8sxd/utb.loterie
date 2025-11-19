using CasinoApp.Application.Interfaces;
using CasinoApp.Domain.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using utb.loterie.Models;

namespace utb.loterie.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IWalletRepository _walletRepository; 

        public AccountController(IUserRepository userRepository, IWalletRepository walletRepository)
        {
            _userRepository = userRepository;
            _walletRepository = walletRepository;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userRepository.GetByUsernameAsync(model.Username);

            if (user == null || user.PasswordHash != model.Password)
            {
                ModelState.AddModelError("", "Neplatné jméno nebo heslo.");
                return View(model);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var claimsIdentity = new ClaimsIdentity(claims, "CookieAuth");
            await HttpContext.SignInAsync("CookieAuth", new ClaimsPrincipal(claimsIdentity));

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var existingUser = await _userRepository.GetByUsernameAsync(model.Username);
            if (existingUser != null)
            {
                ModelState.AddModelError("Username", "Uživatel s tímto jménem již existuje.");
                return View(model);
            }

            var newUser = new User
            {
                Username = model.Username,
                Email = model.Email,
                PasswordHash = model.Password
            };

            var newWallet = new Wallet
            {
                Id = Guid.NewGuid(),
                Currency = "CZK",
                Balance = 50000, // Vstupní bonus :)
                ConcurrencyToken = BitConverter.GetBytes(DateTime.UtcNow.Ticks)
            };

            newUser.Wallet = newWallet; 

            await _userRepository.AddAsync(newUser);

            return await Login(new LoginViewModel { Username = model.Username, Password = model.Password });
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("CookieAuth");
            return RedirectToAction("Index", "Home");
        }
    }
}