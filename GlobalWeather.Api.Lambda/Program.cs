using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using GlobalWeather.Infrastructure;
using GlobalWeather.Api.Lambda.Extensions;
using GlobalWeather.Api.Lambda;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOutputCache();
builder.Services
    .AddApiServices()
    .AddInfrastructure(
        builder.Configuration,
        builder.Environment);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey =
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("JwtKey") ??
                                                            throw new Exception("Missing JWT Key"))),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services.AddAuthorization();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(opt =>
    opt.AddDefaultPolicy(policy => policy
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader()));

// Add AWS Lambda support. When application is run in Lambda Kestrel is swapped out as the web server with Amazon.Lambda.AspNetCoreServer. This
// package will act as the webserver translating request and responses between the Lambda event source and ASP.NET Core.
builder.Services.AddAWSLambdaHosting(LambdaEventSource.RestApi);

var app = builder.Build();
app.UseCors();

if (app.Environment.IsDevelopment())
{
    var database = app.Services.GetRequiredService<DatabaseHelper>();
    await database.CreateTablesAsync(new CancellationTokenSource(TimeSpan.FromSeconds(10)).Token);
}

app.UseHttpsRedirection();
app.UseOutputCache();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("api/v1/health", () => Results.Ok("Healthy"));
app.MapCountryEndpoints()
    .MapWeatherEndpoints()
    .MapUserEndpoints();

app.UseSwagger();
app.UseSwaggerUI();

app.Run();