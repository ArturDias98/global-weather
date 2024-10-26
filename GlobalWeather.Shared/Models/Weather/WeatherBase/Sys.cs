using System.Text.Json.Serialization;

namespace GlobalWeather.Shared.Models.Weather.WeatherBase;

public class Sys
{
    [JsonPropertyName("type")] public int Type { get; set; }

    [JsonPropertyName("id")] public int Id { get; set; }

    [JsonPropertyName("country")] public string Country { get; set; }= string.Empty;

    [JsonPropertyName("sunrise")] public int Sunrise { get; set; }

    [JsonPropertyName("sunset")] public int Sunset { get; set; }
}