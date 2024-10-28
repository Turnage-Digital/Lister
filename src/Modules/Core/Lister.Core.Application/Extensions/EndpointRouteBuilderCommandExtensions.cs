using Lister.Core.Application.Validation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace Lister.Core.Application.Extensions;

public static class EndpointRouteBuilderCommandExtensions
{
    public static IEndpointRouteBuilder MapCreateCommand<TCommand, TResult>(this IEndpointRouteBuilder endpoints,
        string locationTemplate, Func<TResult, object> idSelector)
        where TCommand : IRequest<TResult>
    {
        endpoints.MapPost("/", async (
                TCommand command,
                IMediator mediator,
                IValidator validator) =>
            {
                var validationResult = validator.Validate(command);
                if (validationResult.IsNotValid)
                    return Results.ValidationProblem(validationResult.Errors, statusCode: 500);
                var result = await mediator.Send(command);
                var id = result is null ? string.Empty : idSelector(result);
                var location = string.Format(locationTemplate, id);
                return Results.Created(location, result);
            })
            .Produces(Status201Created)
            .Produces(Status401Unauthorized)
            .Produces<ProblemDetails>(Status500InternalServerError)
            .Produces<HttpValidationProblemDetails>(Status500InternalServerError);
        return endpoints;
    }

    public static IEndpointRouteBuilder MapPostCommand<TCommand>(this IEndpointRouteBuilder endpoints,
        string path)
        where TCommand : IRequest
    {
        endpoints.MapPost(path, HandleCommandAsync<TCommand>)
            .Produces(Status200OK)
            .Produces(Status401Unauthorized)
            .Produces<ProblemDetails>(Status500InternalServerError)
            .Produces<HttpValidationProblemDetails>(Status500InternalServerError);
        return endpoints;
    }

    public static IEndpointRouteBuilder MapPutCommand<TCommand>(this IEndpointRouteBuilder endpoints,
        string path)
        where TCommand : IRequest
    {
        endpoints.MapPut(path, HandleCommandAsync<TCommand>)
            .Produces(Status200OK)
            .Produces(Status401Unauthorized)
            .Produces<ProblemDetails>(Status500InternalServerError)
            .Produces<HttpValidationProblemDetails>(Status500InternalServerError);
        return endpoints;
    }

    public static IEndpointRouteBuilder MapPatchCommand<TCommand>(this IEndpointRouteBuilder endpoints,
        string path)
        where TCommand : IRequest
    {
        endpoints.MapPatch(path, HandleCommandAsync<TCommand>)
            .Produces(Status200OK)
            .Produces(Status401Unauthorized)
            .Produces<ProblemDetails>(Status500InternalServerError)
            .Produces<HttpValidationProblemDetails>(Status500InternalServerError);
        return endpoints;
    }

    private static async Task<IResult> HandleCommandAsync<TCommand>(
        TCommand command,
        IMediator mediator,
        IValidator validator
    ) where TCommand : IRequest
    {
        var validationResult = validator.Validate(command);
        if (validationResult.IsNotValid)
        {
            return Results.ValidationProblem(validationResult.Errors,
                statusCode: Status500InternalServerError);
        }

        await mediator.Send(command);
        return Results.Ok();
    }

    public static IEndpointRouteBuilder MapDeleteCommand<TCommand>(this IEndpointRouteBuilder endpoints,
        string path)
        where TCommand : IRequest
    {
        endpoints.MapDelete(path, async (
                [AsParameters] TCommand command,
                [FromServices] IMediator mediator) =>
            {
                await mediator.Send(command);
                return Results.Ok();
            })
            .Produces(Status200OK)
            .Produces(Status401Unauthorized)
            .Produces<ProblemDetails>(Status500InternalServerError);
        return endpoints;
    }

    public static IEndpointRouteBuilder MapPostCommand<TCommand, TResult>(this IEndpointRouteBuilder endpoints,
        string path)
        where TCommand : IRequest<TResult>
    {
        endpoints.MapPost(path, HandleCommandWithResultAsync<TCommand, TResult>)
            .Produces(Status200OK)
            .Produces(Status401Unauthorized)
            .Produces<ProblemDetails>(Status500InternalServerError)
            .Produces<HttpValidationProblemDetails>(Status500InternalServerError);
        return endpoints;
    }

    public static IEndpointRouteBuilder MapPutCommand<TCommand, TResult>(this IEndpointRouteBuilder endpoints,
        string path)
        where TCommand : IRequest<TResult>
    {
        endpoints.MapPut(path, HandleCommandWithResultAsync<TCommand, TResult>)
            .Produces(Status200OK)
            .Produces(Status401Unauthorized)
            .Produces<ProblemDetails>(Status500InternalServerError)
            .Produces<HttpValidationProblemDetails>(Status500InternalServerError);
        return endpoints;
    }

    public static IEndpointRouteBuilder MapPatchCommand<TCommand, TResult>(this IEndpointRouteBuilder endpoints,
        string path)
        where TCommand : IRequest<TResult>
    {
        endpoints.MapPatch(path, HandleCommandWithResultAsync<TCommand, TResult>)
            .Produces(Status200OK)
            .Produces(Status401Unauthorized)
            .Produces<ProblemDetails>(Status500InternalServerError)
            .Produces<HttpValidationProblemDetails>(Status500InternalServerError);
        return endpoints;
    }

    private static async Task<IResult> HandleCommandWithResultAsync<TCommand, TResult>(
        TCommand command,
        IMediator mediator,
        IValidator validator
    ) where TCommand : IRequest<TResult>
    {
        var validationResult = validator.Validate(command);
        if (validationResult.IsNotValid)
        {
            return Results.ValidationProblem(validationResult.Errors,
                statusCode: Status500InternalServerError);
        }

        var result = await mediator.Send(command);
        return Results.Ok(result);
    }
}