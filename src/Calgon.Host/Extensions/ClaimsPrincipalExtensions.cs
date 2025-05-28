using System.Security.Claims;

namespace Calgon.Host.Extensions;

internal static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal principal)
    {
        var userIdString = principal.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;

        return Guid.Parse(userIdString);
    }
}