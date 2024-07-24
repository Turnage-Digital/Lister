using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Lister.Application.Extensions;

public static class EndpointRouteBuilderQueryExtensions
{
    public static IEndpointRouteBuilder MapGetQuery<TQuery, TResult>(this IEndpointRouteBuilder endpoints, string path)
        where TQuery : IRequest<TResult>
    {
        endpoints.MapGet(path, async (
                [AsParameters] TQuery query,
                [FromServices] IMediator mediator
            ) =>
            {
                var result = await mediator.Send(query);
                return Results.Ok(result);
            })
            .Produces<TResult>()
            .Produces(401)
            .Produces<ProblemDetails>(500);
        return endpoints;
    }
}