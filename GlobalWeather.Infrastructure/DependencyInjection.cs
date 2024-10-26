using GlobalWeather.Infrastructure.Services;
using GlobalWeather.Shared.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GlobalWeather.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddHttpClient<ICountryService, CountryService>(cfg =>
                cfg.BaseAddress = new Uri(configuration.GetValue<string>("RestCountriesUrl")
                                          ?? throw new ArgumentException("Missing Rest Countries address")));

        services.AddHttpClient<IWeatherService, WeatherService>(cfg =>
            cfg.BaseAddress = new Uri(configuration.GetValue<string>("OpenWeatherUrl")
                                      ?? throw new ArgumentException("Missing OpenWeather address")));

        return services;
    }
}