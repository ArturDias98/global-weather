using GlobalWeather.Api.Services;
using GlobalWeather.Domain.Entities;
using GlobalWeather.Domain.Repositories;
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
            .MapGetEndpoints()
            .MapPostEndpoints();
    }

    private static WebApplication MapGetEndpoints(this WebApplication app)
    {
        app.MapGet("api/v1/user/{id}", async (
                [FromRoute] string id,
                [FromServices] IUserService service,
                CancellationToken cancellationToken) =>
            {
                var result = await service.GetUserByIdAsync(id, cancellationToken);
                return result.Success
                    ? Results.Ok(result)
                    : Results.NotFound(result);
            })
            .WithName("get-user-by-id")
            .WithDescription("Get user by id")
            .WithTags("User")
            .Produces<ResultModel<UserModel>>()
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
        return app;
    }

    private static WebApplication MapPostEndpoints(this WebApplication app)
    {
        app.MapPost("api/v1/user/create", async (
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
            .Produces(StatusCodes.Status400BadRequest);

        app.MapPost("api/v1/user/login", async (
                [FromBody] LoginModel model,
                [FromServices] IUserService service,
                [FromServices] TokenService tokenService,
                CancellationToken cancellationToken) =>
            {
                var email = model.Email.Trim();
                var result = await service.LoginAsync(
                    email,
                    model.Password.Trim(),
                    cancellationToken);

                if (!result.Success) return Results.BadRequest(result);

                var token = tokenService.CreateToken(result.Result!, email);

                return Results.Ok(ResultModel<string>.SuccessResult(token));
            })
            .WithName("login")
            .WithDescription("Execute user authentication and return Jwt Token")
            .WithTags("User")
            .Produces<ResultModel<string>>()
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);

        return app;
    }
}