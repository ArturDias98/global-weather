using GlobalWeather.Shared.Contracts;
using GlobalWeather.Shared.Models;

namespace GlobalWeather.Client.Services;

internal class UserService(HttpClient httpClient) : IUserService
{
    public Task<ResultModel<string>> CreateUserAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<ResultModel<string>> LoginAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}