using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Runtime;
using GlobalWeather.Domain.Repositories;
using GlobalWeather.Infrastructure.Options;
using GlobalWeather.Infrastructure.Repositories;
using GlobalWeather.Infrastructure.Services;
using GlobalWeather.Shared.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GlobalWeather.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        services.ConfigureHttpClient(configuration);
        
        return services
            .ConfigureDbContext(configuration, environment)
            .AddTransient<DatabaseHelper>()
            .AddTransient<IUserRepository, UserRepository>()
            .AddTransient<IUserService, UserService>();
    }

    private static void ConfigureHttpClient(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddHttpClient<ICountryService, CountryService>(cfg =>
                cfg.BaseAddress = new Uri(configuration.GetValue<string>("RestCountriesUrl")
                                          ?? throw new ArgumentException("Missing Rest Countries address")));

        services.AddHttpClient<IWeatherService, WeatherService>(cfg =>
            cfg.BaseAddress = new Uri(configuration.GetValue<string>("OpenWeatherUrl")
                                      ?? throw new ArgumentException("Missing OpenWeather address")));
    }

    private static IServiceCollection ConfigureDbContext(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        var options = new AwsDbOptions();
        configuration.Bind(AwsDbOptions.Position, options);

        var config = new AmazonDynamoDBConfig();

        if (environment.IsDevelopment())
        {
            config.ServiceURL = options.ServiceUrl;
        }
        else
        {
            config.RegionEndpoint = RegionEndpoint.USEast1;
        }

        var credentials = new BasicAWSCredentials(options.AccessKey, options.SecretKey);
        var dynamoDbClient = new AmazonDynamoDBClient(credentials, config);

        return services
            .AddSingleton<IAmazonDynamoDB>(dynamoDbClient)
            .AddSingleton<IDynamoDBContext>(new DynamoDBContext(
                dynamoDbClient,
                new DynamoDBContextConfig { Conversion = DynamoDBEntryConversion.V2 }));
    }
}