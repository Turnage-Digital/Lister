using System.Security.Claims;
using Lister.Application.Commands;
using Lister.Application.Queries;
using Lister.Core.SqlDB.Views;
using Lister.Extensions;
using MediatR;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace Lister;

public static class ListsApi
{
    public static IEndpointConventionBuilder MapListsApi(this IEndpointRouteBuilder endpoints)
    {
        var retval = endpoints
            .MapGroup("/api/lists")
            .RequireAuthorization()
            .WithTags("Lists");

        retval.MapGet("/", async (
                IMediator mediator,
                ClaimsPrincipal claimsPrincipal
            ) =>
            {
                var identity = (ClaimsIdentity)claimsPrincipal.Identity!;
                var userId = identity.GetUserId();
                GetListsQuery<ListView> query = new(userId);
                var result = await mediator.Send(query);
                return Results.Ok(result);
            })
            .Produces(Status401Unauthorized)
            .Produces<ListView[]>()
            .Produces(Status500InternalServerError);

        retval.MapGet("/{id}", async (
                Guid id,
                IMediator mediator,
                ClaimsPrincipal claimsPrincipal
            ) =>
            {
                var identity = (ClaimsIdentity)claimsPrincipal.Identity!;
                var userId = identity.GetUserId();
                GetListByIdQuery<ListView> query = new(userId, id);
                var result = await mediator.Send(query);
                return result == null ? Results.NotFound() : Results.Ok(result);
            })
            .Produces(Status401Unauthorized)
            .Produces<ListView>()
            .Produces(Status500InternalServerError);

        retval.MapGet("/names", async (
                IMediator mediator,
                ClaimsPrincipal claimsPrincipal
            ) =>
            {
                var identity = (ClaimsIdentity)claimsPrincipal.Identity!;
                var userId = identity.GetUserId();
                GetListNamesQuery<ListNameView> query = new(userId);
                var result = await mediator.Send(query);
                return Results.Ok(result);
            })
            .Produces(Status401Unauthorized)
            .Produces<ListNameView[]>()
            .Produces(Status500InternalServerError);

        retval.MapPost("/create", async (
                CreateListCommand<ListView> command,
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
            .Produces<ListView>(Status201Created)
            .Produces(Status500InternalServerError);

        retval.MapPatch("/{id}/create-item", async (
                Guid id,
                CreateListItemCommand command,
                IMediator mediator,
                ClaimsPrincipal claimsPrincipal
            ) =>
            {
                var identity = (ClaimsIdentity)claimsPrincipal.Identity!;
                var userId = identity.GetUserId();
                command.CreatedBy = userId;
                command.ListId = id;
                await mediator.Send(command);
                return Results.Ok();
            })
            .Produces(Status401Unauthorized)
            .Produces(Status200OK)
            .Produces(Status500InternalServerError);

        return retval;
    }
}