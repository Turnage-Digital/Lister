using Lister.Application;
using Lister.Application.Commands;
using Lister.Application.Extensions;
using Lister.Application.Queries;
using Lister.Core.Entities;
using Lister.Core.Entities.Views;
using Microsoft.AspNetCore.Identity;

namespace Lister.Server.Extensions;

public static class EndpointRouteBuilderApiExtensions
{
    public static RouteGroupBuilder MapListsApi(this IEndpointRouteBuilder endpoints)
    {
        var retval = endpoints
            .MapGroup("/lists")
            .WithTags("Lists")
            .RequireAuthorization();

        retval.MapCreateCommand<CreateListCommand<ListItemDefinition>, ListItemDefinition>(
            "/{0}", r => r.Id!);
        retval.MapPatchCommand<AddListItemCommand, Item>("{listId}/add");
        retval.MapPostCommand<ConvertTextToListItemCommand, Item>("convert-text-to-list-item");
        retval.MapDeleteCommand<DeleteListCommand>();

        retval.MapGetQuery<GetListItemQuery, Item?>("{listId}/{itemId}");
        retval.MapGetQuery<GetListItemDefinitionQuery<ListItemDefinition>, ListItemDefinition>(
            "{listId}/itemDefinition");
        retval.MapGetQuery<GetListItemsQuery, PagedResponse<Item>>("{listId}");
        retval.MapGetQuery<GetListNamesQuery<ListName>, ListName[]>("names");

        return retval;
    }

    public static RouteGroupBuilder MapIdentityApi(this IEndpointRouteBuilder endpoints)
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