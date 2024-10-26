using GlobalWeather.Shared.Models;

namespace GlobalWeather.Shared.Contracts;

public interface IUserService
{
    Task<ResultModel<string>> CreateUserAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default);

    Task<ResultModel<string>> LoginAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default);
}