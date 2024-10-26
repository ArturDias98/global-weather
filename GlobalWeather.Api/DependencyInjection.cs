using GlobalWeather.Api.Services;

namespace GlobalWeather.Api;

internal static class DependencyInjection
{
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        return services
            .AddSingleton<TokenService>();
    }
}