using Microsoft.AspNetCore.Mvc;
using SampleWebAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add Redis Cache Service
builder.AddRedisClient("sanz-redis");
builder.Services.AddScoped<RedisCacheService>();

// Add Aspire Service Defaults 
builder.AddServiceDefaults();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/test", () => "Hello World!");

app.MapGet("/weatherforecast-redis", async ([FromServices] RedisCacheService redisService) =>
{
    Console.WriteLine("Try to read data from redis...");
    var result = await redisService.GetAsync<WeatherForecast[]>("redis-key");

    if (result is { Length: > 0 })
    {
        Console.WriteLine("*******Got data from redis...");
        return result;
    }

    Console.WriteLine("No data found in the redis...");
    var forecast = Enumerable.Range(1, 5).Select(index =>
            new WeatherForecast
            (
                DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                Random.Shared.Next(-20, 55),
                summaries[Random.Shared.Next(summaries.Length)]
            ))
        .ToArray();
    
    // add sleep for 3seconds
    await Task.Delay(3000);
    
    await redisService.SaveAsync("redis-key", forecast, TimeSpan.FromSeconds(10));

    return forecast;
}).WithName("GetWeatherForecast-Redis");

// Note: Health Check on endpoint
app.MapDefaultEndpoints();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
