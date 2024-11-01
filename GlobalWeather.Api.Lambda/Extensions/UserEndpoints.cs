using GlobalWeather.Api.Lambda.Services;
using GlobalWeather.Shared.Contracts;
using GlobalWeather.Shared.Models;
using GlobalWeather.Shared.Models.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.OpenApi.Models;

namespace GlobalWeather.Api.Lambda.Extensions;

internal static class UserEndpoints
{
    private const string TagName = "User";

    public static WebApplication MapUserEndpoints(this WebApplication app)
    {
        return app
            .MapGetEndpoints()
            .MapPostEndpoints()
            .MapDeleteEndpoints();
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
            //.CacheOutput(cfg => cfg.Expire(TimeSpan.FromMinutes(5)).Tag("get-user"))
            .Produces<ResultModel<UserModel>>(StatusCodes.Status200OK, "application/json")
            .Produces<ResultModel<UserModel>>(StatusCodes.Status404NotFound, "application/json")
            .WithOpenApi(x => new OpenApiOperation(x)
            {
                Description = "Get user using identifier",
                Summary = "Get user by id",
                Tags =
                [
                    new OpenApiTag
                    {
                        Name = TagName
                    }
                ],
                Parameters =
                [
                    new OpenApiParameter
                    {
                        Name = "id",
                        Description = "User identifier",
                        Required = true,
                        In = ParameterLocation.Path
                    },
                ],
            });

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
            .Produces<ResultModel<string>>(StatusCodes.Status200OK, "application/json")
            .Produces<ResultModel<string>>(StatusCodes.Status400BadRequest, "application/json")
            .WithOpenApi(x => new OpenApiOperation(x)
            {
                Description = "Create user and return Jwt Token for authentication",
                Summary = "Create user",
                Tags =
                [
                    new OpenApiTag
                    {
                        Name = TagName
                    }
                ]
            });

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
            .Produces<ResultModel<string>>(StatusCodes.Status200OK, "application/json")
            .Produces<ResultModel<string>>(StatusCodes.Status400BadRequest, "application/json")
            .WithOpenApi(x => new OpenApiOperation(x)
            {
                Description = "Execute user authentication and return Jwt Token",
                Summary = "User login",
                Tags =
                [
                    new OpenApiTag
                    {
                        Name = TagName
                    }
                ]
            });

        app.MapPost("api/v1/user/validate", (
                [FromBody] string token,
                [FromServices] TokenService service) =>
            {
                var isValid = service.ValidateToken(token);

                return ResultModel<bool>.SuccessResult(isValid);
            })
            .WithName("validate-user-token")
            .Produces<ResultModel<bool>>(StatusCodes.Status200OK, "application/json")
            .WithOpenApi(x => new OpenApiOperation(x)
            {
                Description = "Validate user token",
                Summary = "User token validator",
                Tags =
                [
                    new OpenApiTag
                    {
                        Name = TagName
                    }
                ]
            });

        return app;
    }

    private static WebApplication MapDeleteEndpoints(this WebApplication app)
    {
        app.MapDelete("api/v1/user/{id}", async (
                [FromRoute] string id,
                [FromServices] IUserService service,
                [FromServices] IOutputCacheStore outputCacheStore,
                CancellationToken cancellationToken) =>
            {
                var result = await service.DeleteUserAsync(id, cancellationToken);

                if (result.Success)
                {
                    await outputCacheStore.EvictByTagAsync(
                        "get-user",
                        cancellationToken);
                }

                return result.Success
                    ? Results.Ok(result)
                    : Results.NotFound(result);
            })
            .WithName("delete-user")
            .Produces<ResultModel<string>>(StatusCodes.Status200OK, "application/json")
            .Produces<ResultModel<string>>(StatusCodes.Status400BadRequest, "application/json")
            .WithOpenApi(x => new OpenApiOperation(x)
            {
                Description = "Delete user and return user identification",
                Summary = "Delete use by id",
                Tags =
                [
                    new OpenApiTag
                    {
                        Name = TagName
                    }
                ],
                Parameters =
                [
                    new OpenApiParameter
                    {
                        Name = "id",
                        Description = "User identifier",
                        Required = true,
                        In = ParameterLocation.Path
                    },
                ],
            });

        return app;
    }
}