using System.Text.Json.Serialization;

namespace GlobalWeather.Shared.Models.Users;

public class LoginModel
{
    [JsonPropertyName("email")] public required string Email { get; init; } = string.Empty;
    [JsonPropertyName("password")] public required string Password { get; init; } = string.Empty;
}