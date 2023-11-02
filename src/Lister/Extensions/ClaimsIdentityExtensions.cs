using System.Security.Claims;

namespace Lister.Extensions;

public static class ClaimsIdentityExtensions
{
    public static string GetUserId(this ClaimsIdentity identity)
    {
        var retval = identity.Claims
            .Single(claim => claim.Type == ClaimTypes.NameIdentifier)
            .Value;
        return retval;
    }
}