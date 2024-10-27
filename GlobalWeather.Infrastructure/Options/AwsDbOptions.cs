namespace GlobalWeather.Infrastructure.Options;

public class AwsDbOptions
{
    public const string Position = "AwsDbConfiguration";

    public string ServiceUrl { get; set; } = default!;
    public string AccessKey { get; set; } = default!;
    public string SecretKey { get; set; } = default!;
}