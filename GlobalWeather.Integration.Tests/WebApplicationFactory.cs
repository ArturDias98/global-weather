using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Runtime;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.DynamoDb;

namespace GlobalWeather.Integration.Tests;

internal sealed class WebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly DynamoDbContainer _dynamoDbContainer = new DynamoDbBuilder()
        .WithImage("amazon/dynamodb-local:latest")
        .WithName(Guid.NewGuid().ToString())
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll(typeof(IAmazonDynamoDB))
                .RemoveAll(typeof(IDynamoDBContext));

            var connectionString = _dynamoDbContainer.GetConnectionString();

            var config = new AmazonDynamoDBConfig
            {
                ServiceURL = connectionString
            };

            var credentials = new BasicAWSCredentials(
                "testaccesskey",
                "testsecretkey");
            
            var dynamoDbClient = new AmazonDynamoDBClient(credentials, config);
            
            services
                .AddSingleton<IAmazonDynamoDB>(dynamoDbClient)
                .AddSingleton<IDynamoDBContext>(new DynamoDBContext(
                    dynamoDbClient,
                    new DynamoDBContextConfig { Conversion = DynamoDBEntryConversion.V2 }));
        });
    }

    public async Task InitializeAsync()
    {
        await _dynamoDbContainer.StartAsync();
    }

    public new async Task DisposeAsync()
    {
        await _dynamoDbContainer.DisposeAsync();
        await base.DisposeAsync();
    }
}