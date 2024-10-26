using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace GlobalWeather.Api.Services;

internal sealed class TokenService(
    IConfiguration configuration,
    ILogger<TokenService> logger)
{
    private static ClaimsIdentity GenerateClaims(string userId, string email)
    {
        var claims = new List<Claim>()
        {
            new("Id", userId),
            new(ClaimTypes.Email, email)
        };

        return new ClaimsIdentity(claims);
    }

    public string CreateToken(string userId, string email)
    {
        try
        {
            var privateKey = configuration.GetSection("JwtKey").Value ?? throw new Exception();

            var key = Encoding.ASCII.GetBytes(privateKey);

            JwtSecurityTokenHandler handler = new();

            SigningCredentials credentials = new(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                SigningCredentials = credentials,
                Expires = DateTime.UtcNow.AddHours(8),
                Subject = GenerateClaims(userId, email)
            };

            var token = handler.CreateToken(tokenDescriptor);

            return handler.WriteToken(token);
        }
        catch (Exception ex)
        {
            logger.LogError("Error on generate Jwt token for user {user}. Error: {message}",
                userId,
                ex.ToString());

            return string.Empty;
        }
    }

    public static IEnumerable<Claim> GetClaims(string token)
    {
        JwtSecurityTokenHandler handler = new();

        var read = handler.ReadJwtToken(token);

        return read.Claims;
    }

    public bool ValidateToken(string token)
    {
        JwtSecurityTokenHandler handler = new();

        try
        {
            var privateKey = configuration.GetSection("JwtKey").Value ?? throw new Exception();

            handler.ValidateToken(
                token,
                new TokenValidationParameters
                {
                    ValidateLifetime = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(privateKey))
                },
                out _);

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}