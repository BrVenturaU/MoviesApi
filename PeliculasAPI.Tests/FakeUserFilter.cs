using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PeliculasAPI.Tests
{
    public class FakeUserFilter : IAsyncActionFilter
    {
        private string _defaultUserId = "9722b56a-77ea-4e41-941d-e319b6eb3712";
        private string _defaultUserEmail = "ejemplo@hotmail.com";
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var claims = new Claim[]
            {
                new Claim(ClaimTypes.Email, _defaultUserEmail),
                new Claim(ClaimTypes.Name, _defaultUserEmail),
                new Claim(ClaimTypes.NameIdentifier, _defaultUserId)
            };
            context.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(claims, "prueba"));

            await next();
        }
    }
}
