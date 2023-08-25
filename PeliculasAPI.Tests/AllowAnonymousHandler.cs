using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace PeliculasAPI.Tests
{
    public class AllowAnonymousHandler : IAuthorizationHandler
    {
        public Task HandleAsync(AuthorizationHandlerContext context)
        {
            foreach (var requirement in context.Requirements)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
