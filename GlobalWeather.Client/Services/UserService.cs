using System.Net.Http.Json;
using GlobalWeather.Shared.Contracts;
using GlobalWeather.Shared.Models;
using GlobalWeather.Shared.Models.Users;

namespace GlobalWeather.Client.Services;

internal class UserService(
    HttpClient httpClient,
    ILogger<UserService> logger) : IUserService
{
    public async Task<ResultModel<string>> CreateUserAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await httpClient.PostAsJsonAsync(
                "create",
                new CreateUserModel
                {
                    Email = email.Trim(),
                    Password = password.Trim(),
                },
                cancellationToken);

            result.EnsureSuccessStatusCode();

            var content = await result.Content.ReadFromJsonAsync<ResultModel<string>>(cancellationToken);

            return content ?? ResultModel<string>.ErrorResult("Could not read response");
        }
        catch (Exception e)
        {
            logger.LogError("Error on create user. Error: {error}", e.ToString());
            return ResultModel<string>.ErrorResult("Internal Server Error");
        }
    }

    public Task<ResultModel<string>> LoginAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}