using System.Security.Claims;
using Lister.Models;
using Microsoft.AspNetCore.Identity;
using Claim = Lister.Models.Claim;
using static Microsoft.AspNetCore.Http.StatusCodes;

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
            .Produces<SignInResponse>();

        retval.MapGet("/claims", (ClaimsPrincipal claimsPrincipal) =>
            {
                if (claimsPrincipal.Identity?.IsAuthenticated != true)
                {
                    return Results.Unauthorized();
                }

                var claims = claimsPrincipal.Claims
                    .Select(x => new Models.Claim { Type = x.Type, Value = x.Value })
                    .ToArray();
                return Results.Json(new ClaimsResponse { Claims = claims });
            })
            .Produces(Status401Unauthorized)
            .Produces<Models.Claim[]>();

        return retval;
    }
}