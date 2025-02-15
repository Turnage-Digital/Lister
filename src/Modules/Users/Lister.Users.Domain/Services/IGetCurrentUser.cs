using System.Security.Claims;

namespace Lister.Users.Domain.Services;

public interface IGetCurrentUser
{
    ClaimsPrincipal? CurrentUser { get; }
}