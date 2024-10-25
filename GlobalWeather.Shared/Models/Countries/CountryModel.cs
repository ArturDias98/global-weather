using System.Text.Json.Serialization;

namespace GlobalWeather.Shared.Models.Countries;

public class CountryModel
{
    [JsonPropertyName("name")] public CountryName Name { get; set; } = new();

    [JsonPropertyName("capital")] public string[] Capital { get; set; } = [];

    [JsonPropertyName("region")] public string Region { get; set; } = string.Empty;

    [JsonPropertyName("languages")] public Dictionary<string, string> Languages { get; set; } = [];

    [JsonPropertyName("latlng")] public double[] Coordinates { get; set; } = [];

    [JsonPropertyName("population")] public long Population { get; set; }

    [JsonPropertyName("continents")] public string[] Continents { get; set; } = [];

    [JsonPropertyName("flags")] public string[] Flags { get; set; } = [];

    [JsonPropertyName("capitalInfo")] public Dictionary<string, double[]> CapitalInfo { get; set; } = [];
}

public class CountryName
{
    [JsonPropertyName("common")] public string CommonName { get; set; } = string.Empty;
    [JsonPropertyName("official")] public string Official { get; set; } = string.Empty;
}