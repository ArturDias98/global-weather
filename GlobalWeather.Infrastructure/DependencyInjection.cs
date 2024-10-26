using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Runtime;
using GlobalWeather.Domain.Repositories;
using GlobalWeather.Infrastructure.Repositories;
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

        var config = new AmazonDynamoDBConfig
        {
            ServiceURL = "http://localhost:8000"
        };

        var credentials = new BasicAWSCredentials("myAccessKeyId", "secretAccessKey");
        var dynamoDbClient = new AmazonDynamoDBClient(credentials, config);

        return services
            .AddSingleton<IAmazonDynamoDB>(dynamoDbClient)
            .AddSingleton<IDynamoDBContext>(new DynamoDBContext(dynamoDbClient,
                new DynamoDBContextConfig { Conversion = DynamoDBEntryConversion.V2 }))
            .AddTransient<DatabaseHelper>()
            .AddTransient<IUserRepository, UserRepository>()
            .AddTransient<IUserService, UserService>();
    }
}