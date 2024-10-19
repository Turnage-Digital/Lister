using MediatR;
using Serilog;

namespace Lister.Application.Behaviors;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        TResponse retval;
        if (request is RequestBase<TResponse> requestBase)
        {
            Log.Information("Handling {request}",
                new { requestBase.GetType().Name, requestBase.UserId });

            retval = await next();

            Log.Information("Handled {request}",
                new { requestBase.GetType().Name, requestBase.UserId });
        }
        else
        {
            retval = await next();
        }

        return retval;
    }
}