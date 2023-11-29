using System.Security.Claims;
using Lister.Models;
using Microsoft.AspNetCore.Identity;
using static Microsoft.AspNetCore.Http.StatusCodes;
using Claim = Lister.Models.Claim;

namespace Lister;

public static class UsersApi
{
    public static IEndpointConventionBuilder MapUserApi(this IEndpointRouteBuilder endpoints)
    {
        var retval = endpoints
            .MapGroup("/api/users")
            .WithTags("Users");

        retval.MapPost("/sign-in", async (SignInRequest input, SignInManager<IdentityUser> signInManager) =>
            {
                var result = await signInManager.PasswordSignInAsync(input.Username, input.Password,
                    true, false);
                return Results.Json(new SignInResponse { Succeeded = result.Succeeded });
            })
            .Produces<SignInResponse>()
            .AllowAnonymous();

        retval.MapPost("/sign-out", async (SignInManager<IdentityUser> signInManager) =>
            {
                await signInManager.SignOutAsync();
                return Results.Ok();
            })
            .Produces(Status200OK)
            .Produces(Status401Unauthorized)
            .RequireAuthorization();

        retval.MapGet("/claims", (ClaimsPrincipal claimsPrincipal) =>
            {
                var claims = claimsPrincipal.Claims
                    .Select(x => new Claim { Type = x.Type, Value = x.Value })
                    .ToArray();
                return Results.Json(new ClaimsResponse { Claims = claims });
            })
            .Produces<Claim[]>()
            .Produces(Status401Unauthorized)
            .RequireAuthorization();

        return retval;
    }
}