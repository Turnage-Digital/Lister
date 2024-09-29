using System.Security.Claims;
using Lister.Application;
using Lister.Server.Extensions;
using MediatR;

namespace Lister.Server.Behaviors;

public class AssignUserBehavior<TRequest, TResponse>(IHttpContextAccessor httpContextAccessor)
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
        var user = httpContextAccessor.HttpContext!.User;
        var identity = (ClaimsIdentity)user.Identity!;
        var retval = identity.GetUserId();
        return retval;
    }
}