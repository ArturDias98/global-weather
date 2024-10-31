using FluentValidation;
using GlobalWeather.Domain.Entities;
using GlobalWeather.Domain.Helpers;
using GlobalWeather.Domain.Repositories;
using GlobalWeather.Shared.Contracts;
using GlobalWeather.Shared.Models;
using GlobalWeather.Shared.Models.Users;
using Microsoft.Extensions.Logging;

namespace GlobalWeather.Infrastructure.Services;

internal sealed class UserService(
    IUserRepository userRepository,
    ILogger<UserService> logger) : IUserService
{
    public async Task<ResultModel<UserModel>> GetUserByIdAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var get = await userRepository.GetUserByIdAsync(userId, cancellationToken);
            
            if (get is null)
            {
                return ResultModel<UserModel>.ErrorResult("Could not find user by id");
            }
            
            var parse = new UserModel
            {
                Id = get.Id,
                Email = get.Email,
                FavoriteCountries = get.FavoriteCountries,
                FavoriteCities = get.FavoriteCities.Select(i => new FavoriteCityModel
                {
                    Id = i.Id,
                    Name = i.Name,
                    Country = i.Country,
                    State = i.State,
                    Latitude = i.Latitude,
                    Longitude = i.Longitude
                }).ToList(),
            };

            return ResultModel<UserModel>.SuccessResult(parse);
        }
        catch (Exception e)
        {
            logger.LogError("Error on get user by id {id}. Error: {message}",
                userId,
                e.ToString());
            return ResultModel<UserModel>.ErrorResult("User not found");
        }
    }

    public async Task<ResultModel<string>> CreateUserAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var validate = await new UserServiceValidator(userRepository)
                .ValidateAsync(new ValueTuple<string, string>(email, password), cancellationToken);

            if (!validate.IsValid)
            {
                return ResultModel<string>.ErrorResult("Invalid user parameters for creation",
                    validate.Errors.Select(i => i.ErrorMessage).ToList());
            }

            PasswordHelper.CreatePasswordHash(
                password,
                out var hash,
                out var salt);

            var user = User.Create(
                email,
                hash,
                salt);

            await userRepository
                .SaveUserAsync(user, cancellationToken)
                .ConfigureAwait(false);

            return ResultModel<string>.SuccessResult(user.Id);
        }
        catch (Exception e)
        {
            logger.LogError("Error on create user entity. Error: {message}", e.ToString());
            return ResultModel<string>.ErrorResult("Error on create user");
        }
    }

    public async Task<ResultModel<string>> DeleteUserAsync(string id, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await userRepository.GetUserByIdAsync(id, cancellationToken);
            
            if (user is null)
            {
                return ResultModel<string>.ErrorResult("User not available for deletion");
            }
            
            await userRepository.DeleteUserAsync(id, cancellationToken);
            return ResultModel<string>.SuccessResult(id);
        }
        catch (Exception e)
        {
            logger.LogError("Error on delete user {id}. Error: {error}",
                id,
                e.ToString());
            return ResultModel<string>.ErrorResult("Error on delete user");
        }
    }

    public async Task<ResultModel<string>> LoginAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await userRepository.GetUserByEmailAsync(email, cancellationToken);

            return !PasswordHelper.VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt)
                ? ResultModel<string>.ErrorResult("Verify email and password")
                : ResultModel<string>.SuccessResult(user.Id);
        }
        catch (Exception e)
        {
            logger.LogError("Error on execute login for user {user}. Error: {message}",
                email,
                e.ToString());

            return ResultModel<string>.ErrorResult("Could not execute login");
        }
    }
}

internal sealed class UserServiceValidator : AbstractValidator<(string Email, string Password)>
{
    public UserServiceValidator(IUserRepository userRepository)
    {
        RuleFor(i => i.Email)
            .NotNull()
            .NotEmpty()
            .EmailAddress()
            .WithMessage("Invalid email address");

        RuleFor(i => i.Password)
            .NotNull()
            .NotEmpty()
            .MinimumLength(6)
            .WithMessage("Password must be at least 6 characters long");

        RuleFor(i => i.Email)
            .MustAsync(async (email, token) =>
            {
                var ensure = email.Trim();

                return string.IsNullOrEmpty(ensure) || await userRepository.IsEmailUniqueAsync(ensure, token);
            })
            .WithMessage("Email is being used");
    }
}