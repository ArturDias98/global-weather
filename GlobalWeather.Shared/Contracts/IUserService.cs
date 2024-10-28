using GlobalWeather.Shared.Models;
using GlobalWeather.Shared.Models.Users;

namespace GlobalWeather.Shared.Contracts;

public interface IUserService
{
    Task<ResultModel<UserModel>> GetUserByIdAsync(
        string userId,
        CancellationToken cancellationToken = default);

    Task<ResultModel<string>> CreateUserAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default);

    Task<ResultModel<string>> LoginAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default);
}