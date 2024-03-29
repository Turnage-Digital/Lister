using MediatR;
using Serilog;

namespace Lister;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        var requestBase = request as RequestBase<TResponse>;
        if (requestBase is not null)
        {
            Log.Information("Handling {request}",
                new { requestBase.RequestId, requestBase.UserId, requestBase.GetType().Name });
        }

        var retval = await next();
        if (requestBase is not null)
        {
            Log.Information("Handled {request}",
                new { requestBase.RequestId, requestBase.UserId, requestBase.GetType().Name });
        }

        return retval;
    }
}