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
        var identity = (ClaimsIdentity)user!.Identity!;
        var retval = identity.GetUserId();
        return retval;
    }
}