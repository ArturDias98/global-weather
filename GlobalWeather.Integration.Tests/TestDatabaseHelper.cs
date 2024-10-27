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
}