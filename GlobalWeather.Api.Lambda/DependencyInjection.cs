using GlobalWeather.Api.Lambda.Services;

namespace GlobalWeather.Api.Lambda;

internal static class DependencyInjection
{
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        return services
            .AddSingleton<TokenService>();
    }
}