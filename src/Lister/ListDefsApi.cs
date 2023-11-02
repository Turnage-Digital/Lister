using System.Security.Claims;
using Lister.Application.Commands;
using Lister.Application.Queries;
using Lister.Core.SqlDB.Views;
using Lister.Extensions;
using MediatR;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace Lister;

public static class ListDefsApi
{
    public static IEndpointConventionBuilder MapListDefsApi(this IEndpointRouteBuilder endpoints)
    {
        var retval = endpoints
            .MapGroup("/api/list-defs")
            .RequireAuthorization();

        retval.MapGet("", async (
                IMediator mediator,
                ClaimsPrincipal claimsPrincipal
            ) =>
            {
                var identity = (ClaimsIdentity)claimsPrincipal.Identity!;
                var userId = identity.GetUserId();
                GetListDefsQuery<ListDefView> query = new(userId);
                var result = await mediator.Send(query);
                return Results.Ok(result);
            })
            .Produces(Status401Unauthorized)
            .Produces<ListDefView[]>()
            .Produces(Status500InternalServerError);

        retval.MapGet("/{id}", async (
                Guid id,
                IMediator mediator,
                ClaimsPrincipal claimsPrincipal
            ) =>
            {
                var identity = (ClaimsIdentity)claimsPrincipal.Identity!;
                var userId = identity.GetUserId();
                GetListDefByIdQuery<ListDefView> query = new(userId, id);
                var result = await mediator.Send(query);
                return Results.Ok(result);
            })
            .Produces(Status401Unauthorized)
            .Produces<ListDefView>()
            .Produces(Status500InternalServerError);

        retval.MapPost("/create", async (
                CreateListDefCommand<ListDefView> command,
                IMediator mediator,
                ClaimsPrincipal claimsPrincipal
            ) =>
            {
                var identity = (ClaimsIdentity)claimsPrincipal.Identity!;
                var userId = identity.GetUserId();
                command.CreatedBy = userId;
                var result = await mediator.Send(command);
                return Results.Created($"/{result.Id}", result);
            })
            .Produces(Status401Unauthorized)
            .Produces<ListDefView>(Status201Created)
            .Produces(Status500InternalServerError);

        return retval;
    }
}