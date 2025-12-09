using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace utb.loterie.Controllers
{

    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}