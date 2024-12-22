using System.Security.Claims;

namespace Lister.Core.Application.Services;

public interface IGetCurrentUser
{
    ClaimsPrincipal CurrentUser { get; }
}