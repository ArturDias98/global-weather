using GlobalWeather.Client.Services;
using GlobalWeather.Shared.Contracts;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace GlobalWeather.Client;

internal static class DependencyInjection
{
    private const string LocalServerUrl = "https://localhost:51343/";
    private const string RemoteServerUrl = "https://g7dyc2vjj5.execute-api.us-east-1.amazonaws.com/Prod/";

    private static string GetServerUrl(IWebAssemblyHostEnvironment environment)
    {
        return environment.IsDevelopment()
            ? LocalServerUrl
            : RemoteServerUrl;
    }

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