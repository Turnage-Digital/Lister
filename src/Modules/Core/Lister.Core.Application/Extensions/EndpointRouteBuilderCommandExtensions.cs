using Lister.Core.Application.Validation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace Lister.Core.Application.Extensions;

public static class EndpointRouteBuilderCommandExtensions
{
    public static IEndpointRouteBuilder MapCreateCommand<TCommand, TResult>(
        this IEndpointRouteBuilder endpoints,
        string path,
        string locationTemplate,
        params Func<TResult, object>[] idSelectors)
        where TCommand : IRequest<TResult>
    {
        endpoints.MapPost(path, async (
                TCommand command,
                IMediator mediator,
                IValidator validator) =>
            {
                IResult retval;

                try
                {
                    var validationResult = validator.Validate(command);
                    if (validationResult.IsNotValid)
                    {
                        retval = Results.ValidationProblem(validationResult.Errors,
                            statusCode: Status500InternalServerError);
                    }
                    else
                    {
                        var result = await mediator.Send(command);
                        var ids = idSelectors.Select(selector => selector(result))
                            .ToArray();
                        var location = string.Format(locationTemplate, ids);
                        retval = Results.Created(location, result);
                    }
                }
                catch (Exception e)
                {
                    retval = Results.Problem(e.Message,
                        statusCode: Status500InternalServerError);
                }

                return retval;
            })
            .Produces(Status201Created)
            .Produces(Status401Unauthorized)
            .Produces<ProblemDetails>(Status500InternalServerError)
            .Produces<HttpValidationProblemDetails>(Status500InternalServerError);
        return endpoints;
    }

    public static IEndpointRouteBuilder MapPostCommand<TCommand>(this IEndpointRouteBuilder endpoints, string path)
        where TCommand : IRequest
    {
        endpoints.MapPost(path, HandleCommandAsync<TCommand>)
            .Produces(Status200OK)
            .Produces(Status401Unauthorized)
            .Produces<ProblemDetails>(Status500InternalServerError)
            .Produces<HttpValidationProblemDetails>(Status500InternalServerError);
        return endpoints;
    }

    public static IEndpointRouteBuilder MapPutCommand<TCommand>(this IEndpointRouteBuilder endpoints, string path)
        where TCommand : IRequest
    {
        endpoints.MapPut(path, HandleCommandAsync<TCommand>)
            .Produces(Status200OK)
            .Produces(Status401Unauthorized)
            .Produces<ProblemDetails>(Status500InternalServerError)
            .Produces<HttpValidationProblemDetails>(Status500InternalServerError);
        return endpoints;
    }

    public static IEndpointRouteBuilder MapPatchCommand<TCommand>(this IEndpointRouteBuilder endpoints, string path)
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
        IResult retval;

        try
        {
            var validationResult = validator.Validate(command);
            if (validationResult.IsNotValid)
            {
                retval = Results.ValidationProblem(validationResult.Errors,
                    statusCode: Status500InternalServerError);
            }
            else
            {
                await mediator.Send(command);
                retval = Results.Ok();
            }
        }
        catch (Exception e)
        {
            retval = Results.Problem(e.Message,
                statusCode: Status500InternalServerError);
        }

        return retval;
    }

    public static IEndpointRouteBuilder MapDeleteCommand<TCommand>(this IEndpointRouteBuilder endpoints, string path)
        where TCommand : IRequest
    {
        endpoints.MapDelete(path, async (
                [AsParameters] TCommand command,
                [FromServices] IMediator mediator) =>
            {
                IResult retval;

                try
                {
                    await mediator.Send(command);
                    retval = Results.Ok();
                }
                catch (Exception e)
                {
                    retval = Results.Problem(e.Message,
                        statusCode: Status500InternalServerError);
                }

                return retval;
            })
            .Produces(Status200OK)
            .Produces(Status401Unauthorized)
            .Produces<ProblemDetails>(Status500InternalServerError);
        return endpoints;
    }

    public static IEndpointRouteBuilder MapPostCommand<TCommand, TResult>(
        this IEndpointRouteBuilder endpoints,
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

    public static IEndpointRouteBuilder MapPutCommand<TCommand, TResult>(
        this IEndpointRouteBuilder endpoints,
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

    public static IEndpointRouteBuilder MapPatchCommand<TCommand, TResult>(
        this IEndpointRouteBuilder endpoints,
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
        IResult retval;

        try
        {
            var validationResult = validator.Validate(command);
            if (validationResult.IsNotValid)
            {
                retval = Results.ValidationProblem(validationResult.Errors,
                    statusCode: Status500InternalServerError);
            }
            else
            {
                var result = await mediator.Send(command);
                retval = Results.Ok(result);
            }
        }
        catch (Exception e)
        {
            retval = Results.Problem(e.Message,
                statusCode: Status500InternalServerError);
        }

        return retval;
    }
}