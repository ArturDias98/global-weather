using Amazon.DynamoDBv2;
using GlobalWeather.Domain.Entities;
using GlobalWeather.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace GlobalWeather.Infrastructure.Repositories;

internal sealed class UserRepository(
    IAmazonDynamoDB db,
    ILogger<UserRepository> logger) : IUserRepository
{
    public Task<User> GetUserByIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task SaveUserAsync(User user, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}