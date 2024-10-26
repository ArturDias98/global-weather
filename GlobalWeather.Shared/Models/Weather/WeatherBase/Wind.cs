using System.Text.Json.Serialization;

namespace GlobalWeather.Shared.Models.Weather.WeatherBase;

public class Wind
{
    [JsonPropertyName("speed")] public double Speed { get; set; }

    [JsonPropertyName("deg")] public int Deg { get; set; }

    [JsonPropertyName("gust")] public double Gust { get; set; }
}