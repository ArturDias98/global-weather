using GlobalWeather.Api.Services;
using GlobalWeather.Shared.Contracts;
using GlobalWeather.Shared.Models;
using GlobalWeather.Shared.Models.Users;
using Microsoft.AspNetCore.Mvc;

namespace GlobalWeather.Api.Extensions;

internal static class UserEndpoints
{
    public static WebApplication MapUserEndpoints(this WebApplication app)
    {
        return app
            .MapPostEndpoints();
    }

    private static WebApplication MapPostEndpoints(this WebApplication app)
    {
        app.MapPost("api/v1/user", async (
            [FromBody] CreateUserModel model,
            [FromServices] IUserService service,
            [FromServices] TokenService tokenService,
            CancellationToken cancellationToken) =>
        {
            var email = model.Email.Trim();
            var result = await service.CreateUserAsync(
                email,
                model.Password.Trim(),
                cancellationToken);

            if (!result.Success) return Results.BadRequest(result);

            var token = tokenService.CreateToken(result.Result!, email);

            return Results.Ok(ResultModel<string>.SuccessResult(token));
        })
        .WithName("create-user")
        .WithDescription("Create user and return Jwt Token for authentication")
        .WithTags("User")
        .Produces<ResultModel<string>>()
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        return app;
    }
}