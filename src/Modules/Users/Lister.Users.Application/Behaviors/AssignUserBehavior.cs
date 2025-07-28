using System.Security.Claims;
using Lister.Core.Application;
using Lister.Users.Application.Extensions;
using Lister.Users.Domain.Services;
using MediatR;

namespace Lister.Users.Application.Behaviors;

public class AssignUserBehavior<TRequest, TResponse>(IGetCurrentUser getter)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        if (request is RequestBase<TResponse> requestBaseWithResponse)
        {
            var userId = GetUserId();
            requestBaseWithResponse.UserId = userId;
        }

        if (request is RequestBase requestBase)
        {
            var userId = GetUserId();
            requestBase.UserId = userId;
        }

        var retval = await next();
        return retval;
    }

    private string GetUserId()
    {
        var user = getter.CurrentUser;

        if (user?.Identity is null)
            throw new InvalidOperationException("No authenticated user found. User authentication is required.");

        if (!user.Identity.IsAuthenticated)
            throw new InvalidOperationException(
                "User is not authenticated. Authentication is required to access this resource.");

        if (user.Identity is not ClaimsIdentity identity)
            throw new InvalidOperationException(
                "User identity is not a ClaimsIdentity. Unable to extract user information.");
        
        var retval = identity.GetUserId();
        return retval;
    }
}