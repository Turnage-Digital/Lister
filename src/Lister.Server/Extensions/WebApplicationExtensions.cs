using Lister.Core.Application.Extensions;
using Lister.Core.Domain;
using Lister.Lists.Application.Commands;
using Lister.Lists.Application.Queries;
using Lister.Lists.Domain.Entities;
using Lister.Lists.Domain.Views;
using Microsoft.AspNetCore.Identity;

namespace Lister.Server.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication UseApplication(this WebApplication app)
    {
        app.MapGroup("/api")
            .MapListsApi();
        app.MapIdentityApi();
        return app;
    }

    private static RouteGroupBuilder MapListsApi(this IEndpointRouteBuilder endpoints)
    {
        var retval = endpoints
            .MapGroup("/lists")
            .WithTags("Lists")
            .RequireAuthorization();

        retval.MapCreateCommand<CreateListCommand, ListItemDefinition>(
            "/{0}", r => r.Id!);
        retval.MapPatchCommand<AddListItemCommand, Item>("{listId}/add");
        retval.MapPostCommand<ConvertTextToListItemCommand, Item>("convert-text-to-list-item");
        retval.MapDeleteCommand<DeleteListCommand>("{listId}");

        retval.MapGetQuery<GetListItemQuery, Item?>("{listId}/{itemId}");
        retval.MapGetQuery<GetListItemDefinitionQuery, ListItemDefinition?>(
            "{listId}/itemDefinition");
        retval.MapGetQuery<GetListItemsQuery, PagedResponse<Item>>("{listId}");
        retval.MapGetQuery<GetListNamesQuery, ListName[]>("names");

        return retval;
    }

    private static RouteGroupBuilder MapIdentityApi(this IEndpointRouteBuilder endpoints)
    {
        var retval = endpoints
            .MapGroup("/identity")
            .WithTags("Identity");

        retval.MapIdentityApi<IdentityUser>();

        retval.MapPost("logout",
            async (SignInManager<IdentityUser> signInManager) =>
            {
                await signInManager.SignOutAsync();
                return Results.Ok();
            }
        );

        return retval;
    }
}