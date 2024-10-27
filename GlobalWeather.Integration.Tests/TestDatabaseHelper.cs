using GlobalWeather.Domain.Entities;
using GlobalWeather.Domain.Helpers;
using GlobalWeather.Domain.Repositories;
using GlobalWeather.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace GlobalWeather.Integration.Tests;

internal static class TestDatabaseHelper
{
    public static async Task MigrateDatabaseAsync(WebApplicationFactory factory)
    {
        await factory.InitializeAsync();

        var helper = factory.Services.GetRequiredService<DatabaseHelper>();
        await helper.CreateTablesAsync(new CancellationTokenSource(TimeSpan.FromSeconds(10)).Token);
    }

    public static async Task<User> CreateUserAsync(
        string email,
        string password,
        WebApplicationFactory factory)
    {
        PasswordHelper.CreatePasswordHash(
            password,
            out var hash,
            out var salt);

        var user = User.Create(
            email,
            hash,
            salt);

        var repository = factory.Services.GetRequiredService<IUserRepository>();
        await repository.SaveUserAsync(user);

        return user;
    }
}