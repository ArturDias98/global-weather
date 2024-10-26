using System.Text.Json.Serialization;
using GlobalWeather.Shared.Models.Weather.WeatherBase;

namespace GlobalWeather.Shared.Models.Weather;

public class WeatherModel
{
    [JsonPropertyName("coord")] public Coordinates Coordinates { get; set; } = new();

    [JsonPropertyName("weather")] public List<WeatherBase.Weather> Weather { get; set; } = [];

    [JsonPropertyName("base")] public string Base { get; set; }= string.Empty;

    [JsonPropertyName("main")] public Main Main { get; set; } = new();

    [JsonPropertyName("visibility")] public int Visibility { get; set; }

    [JsonPropertyName("wind")] public Wind Wind { get; set; } = new();

    [JsonPropertyName("rain")] public Dictionary<string, double> Rain { get; set; } = [];

    [JsonPropertyName("clouds")] public Dictionary<string, double> Clouds { get; set; } = [];

    [JsonPropertyName("dt")] public int Dt { get; set; }

    [JsonPropertyName("sys")] public Sys Sys { get; set; } = new();

    [JsonPropertyName("timezone")] public int Timezone { get; set; }

    [JsonPropertyName("id")] public int Id { get; set; }

    [JsonPropertyName("name")] public string Name { get; set; }= string.Empty;

    [JsonPropertyName("cod")] public int Cod { get; set; }
}