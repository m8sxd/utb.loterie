using CasinoApp.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace utb.loterie.Filters
{
    public class GameBanFilter : ActionFilterAttribute
    {
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var userManager = context.HttpContext.RequestServices.GetService<UserManager<User>>();
            var userPrincipal = context.HttpContext.User;

            if (userPrincipal.Identity.IsAuthenticated)
            {
                var user = await userManager.GetUserAsync(userPrincipal);

                if (user != null && await userManager.IsLockedOutAsync(user))
                {
                    if (IsAjaxRequest(context.HttpContext.Request))
                    {
                        context.Result = new JsonResult(new { message = "Váš účet má zákaz hraní her." }) { StatusCode = 403 };
                    }
                    else
                    {
                        context.Result = new RedirectToActionResult("Banned", "Home", null);
                    }
                    return; 
                }
            }

            await next();
        }

        private bool IsAjaxRequest(HttpRequest request)
        {
            return request.Headers["X-Requested-With"] == "XMLHttpRequest" ||
                   request.Headers["Content-Type"].ToString().Contains("application/json");
        }
    }
}