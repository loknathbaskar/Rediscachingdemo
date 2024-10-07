using Microsoft.AspNetCore.Mvc;
using RedisCachingDemo.Services;
using System.Text.Json;

namespace RedisCachingDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly CacheService _cacheService;

        public WeatherForecastController(CacheService cacheService)
        {
            _cacheService = cacheService;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public async Task<IActionResult> Get()
        {
            string cacheKey = "weatherForecast";

            // Check Redis cache first
            var cachedForecast = await _cacheService.GetCachedValueAsync(cacheKey);

            if (!string.IsNullOrEmpty(cachedForecast))
            {
                return Ok(JsonSerializer.Deserialize<WeatherForecast[]>(cachedForecast));
            }

            // If no cache, generate the forecast
            var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    Summaries[Random.Shared.Next(Summaries.Length)]
                ))
                .ToArray();

            // Store in Redis cache
            var forecastJson = JsonSerializer.Serialize(forecast);
            await _cacheService.SetCacheValueAsync(cacheKey, forecastJson);

            return Ok(forecast);
        }
    }

    public record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
    {
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    }
}
