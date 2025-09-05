using System.Security.Claims;
using Lister.Users.Domain.Services;

namespace Lister.App.Server.Services;

public class CurrentUserGetter(IHttpContextAccessor httpContextAccessor) : IGetCurrentUser
{
    public ClaimsPrincipal? CurrentUser { get; } = httpContextAccessor.HttpContext?.User;
}