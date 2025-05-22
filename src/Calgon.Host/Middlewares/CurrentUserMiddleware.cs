using System.Security.Claims;
using Calgon.Host.Interfaces;

namespace Calgon.Host.Middlewares;

public class CurrentUserMiddleware(ICurrentUserService currentUserService) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (context.User is null || context.User.Identity is null || !context.User.Identity.IsAuthenticated)
        {
            await next(context);
            return;
        }

        currentUserService.CurrentUserId = context.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
        
        await next(context);
    }
}