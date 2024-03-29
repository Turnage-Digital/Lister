using System.Security.Claims;
using Lister.Extensions;
using MediatR;

namespace Lister;

public class AssignUserBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AssignUserBehavior(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        if (request is RequestBase<TResponse> requestBase)
        {
            var user = _httpContextAccessor.HttpContext!.User;
            var identity = (ClaimsIdentity)user.Identity!;
            var userId = identity.GetUserId();
            requestBase.UserId = userId;
        }

        var retval = await next();
        return retval;
    }
}