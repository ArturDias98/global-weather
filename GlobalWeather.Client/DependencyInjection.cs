using GlobalWeather.Client.Services;
using GlobalWeather.Shared.Contracts;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using static GlobalWeather.Client.ServerHelper;

namespace GlobalWeather.Client;

internal static class DependencyInjection
{
    public static IServiceCollection AddClientServices(
        this IServiceCollection services,
        IWebAssemblyHostEnvironment environment)
    {
        services.AddHttpClient<ICountryService, CountryService>(client =>
        {
            var url = GetServerUrl(environment) + "api/v1/country/";
            client.BaseAddress = new Uri(url);
        });
        services.AddHttpClient<IUserService, UserService>(client =>
        {
            var url = GetServerUrl(environment) + "api/v1/user/";
            client.BaseAddress = new Uri(url);
        });
        services.AddHttpClient<IWeatherService, WeatherService>(client =>
        {
            var url = GetServerUrl(environment) + "api/v1/weather/";
            client.BaseAddress = new Uri(url);
        });

        return services
            .AddScoped<AuthService>()
            .AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>()
            .AddAuthorizationCore();
    }
}