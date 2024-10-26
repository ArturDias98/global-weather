using GlobalWeather.Domain.Entities;

namespace GlobalWeather.Domain.Repositories;

public interface IUserRepository
{
    Task<bool> IsEmailUniqueAsync(string email, CancellationToken cancellationToken = default);
    Task<User> GetUserByIdAsync(string userId, CancellationToken cancellationToken = default);
    Task<User> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task DeleteUserAsync(string id, CancellationToken cancellationToken = default);
    Task SaveUserAsync(User user, CancellationToken cancellationToken = default);
}