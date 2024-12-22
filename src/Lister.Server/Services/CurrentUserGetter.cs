using System.Security.Claims;
using Lister.Users.Domain.Services;

namespace Lister.Server.Services;

public class CurrentUserGetter : IGetCurrentUser
{
    public CurrentUserGetter(IHttpContextAccessor httpContextAccessor)
    {
        CurrentUser = httpContextAccessor.HttpContext?.User;
    }

    public ClaimsPrincipal? CurrentUser { get; }
}