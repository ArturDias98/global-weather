using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using GlobalWeather.Domain.Entities;
using GlobalWeather.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace GlobalWeather.Infrastructure.Repositories;

internal sealed class UserRepository(IDynamoDBContext context) : IUserRepository
{
    public async Task<bool> IsEmailUniqueAsync(string email, CancellationToken cancellationToken = default)
    {
        var scan = new ScanCondition("Email", ScanOperator.Equal, email);

        var users = await context
            .ScanAsync<User>([scan])
            .GetRemainingAsync(cancellationToken);
        
        return users?.Count == 0;
    }

    public Task<User?> GetUserByIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        return context.LoadAsync<User?>(userId, cancellationToken);
    }

    public async Task<User> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var scan = new ScanCondition("Email", ScanOperator.Equal, email);
        
        var users = await context
            .ScanAsync<User>([scan])
            .GetRemainingAsync(cancellationToken);
        
        return users.Single();
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