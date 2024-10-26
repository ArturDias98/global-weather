using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

namespace GlobalWeather.Infrastructure;

public class DatabaseHelper(IAmazonDynamoDB client)
{
    public async Task CreateTablesAsync(CancellationToken cancellationToken)
    {
        var listTable = new List<string>()
        {
            "User",
        };

        foreach (var tableName in listTable)
        {
            try
            {
                var tableResponse = await client.DescribeTableAsync(tableName, cancellationToken)
                    .ConfigureAwait(false);
            }
            catch (ResourceNotFoundException)
            {
                var createTableRequest = new CreateTableRequest
                {
                    TableName = tableName,
                    AttributeDefinitions = [new AttributeDefinition("Id", ScalarAttributeType.S)],
                    KeySchema = [new KeySchemaElement("Id", KeyType.HASH)],
                    ProvisionedThroughput = new ProvisionedThroughput
                    {
                        ReadCapacityUnits = 5,
                        WriteCapacityUnits = 5
                    }
                };

                await client.CreateTableAsync(createTableRequest, cancellationToken)
                    .ConfigureAwait(false);

                var tableStatus = "CREATING";
                while (tableStatus == "CREATING")
                {
                    await Task.Delay(1000, cancellationToken).ConfigureAwait(false);
                    var response = await client.DescribeTableAsync(tableName, cancellationToken).ConfigureAwait(false);
                    tableStatus = response.Table.TableStatus;
                }
            }
            catch (Exception)
            {
                //ignore
            }
        }
    }
}