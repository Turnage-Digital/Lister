using System.Security.Claims;
using Lister.Extensions;
using MediatR;

namespace Lister.Behaviors;

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
        if (request is RequestBase<TResponse> requestBase)
        {
            var user = httpContextAccessor.HttpContext!.User;
            var identity = (ClaimsIdentity)user.Identity!;
            var userId = identity.GetUserId();
            requestBase.UserId = userId;
        }

        var retval = await next();
        return retval;
    }
}