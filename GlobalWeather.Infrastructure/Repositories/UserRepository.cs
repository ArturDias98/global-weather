using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using GlobalWeather.Domain.Entities;
using GlobalWeather.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace GlobalWeather.Infrastructure.Repositories;

internal sealed class UserRepository(IDynamoDBContext context) : IUserRepository
{
    public async Task<bool> IsEmailUniqueAsync(string email, CancellationToken cancellationToken = default)
    {
        //TODO: Improve this query
        var users = await context
            .ScanAsync<User>(new List<ScanCondition>())
            .GetRemainingAsync(cancellationToken);
        
        return users.All(i => i.Email != email);
    }

    public Task<User> GetUserByIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        return context.LoadAsync<User>(userId, cancellationToken);
    }

    public async Task<User> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        //TODO: Improve this query
        var users = await context
            .ScanAsync<User>(new List<ScanCondition>())
            .GetRemainingAsync(cancellationToken);
        
        return users.First(i => i.Email == email);
    }

    public Task DeleteUserAsync(string id, CancellationToken cancellationToken = default)
    {
        return context.DeleteAsync<User>(id, cancellationToken);
    }

    public Task SaveUserAsync(User user, CancellationToken cancellationToken = default)
    {
        return context.SaveAsync(user, cancellationToken);
    }
}