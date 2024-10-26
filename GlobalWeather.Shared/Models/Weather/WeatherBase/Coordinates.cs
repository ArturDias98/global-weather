using System.Text.Json.Serialization;

namespace GlobalWeather.Shared.Models.Weather.WeatherBase;

public class Coordinates
{
    [JsonPropertyName("lon")] public double Lon { get; set; }

    [JsonPropertyName("lat")] public double Lat { get; set; }
}