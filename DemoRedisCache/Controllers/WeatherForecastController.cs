using DemoRedisCache.Attributes;
using DemoRedisCache.Context;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System.Diagnostics.Metrics;
using System.Text;

namespace DemoRedisCache.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly IRedisConnectionMultiplexer _redisConnectionMultiplexer;

        public WeatherForecastController(
            IRedisConnectionMultiplexer redisConnectionMultiplexer)
        {
            _redisConnectionMultiplexer = redisConnectionMultiplexer;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IActionResult Get()
        {
            var db = _redisConnectionMultiplexer.GetDatabase();

            for (int i = 1; i < 100; i++)
            {
                db.HashSet(
                    $"person:{i}",
                    new HashEntry[]
                    {
                        new HashEntry("firstName", GenerateRandomString()),
                        new HashEntry("lastName", GenerateRandomString())
                    });
                db.SortedSetAdd("followers", $"{i}", (double)GenerateRandomNumber());
            }
            //var allFields = db.HashGetAll("person:1");
            return Ok();
        }

        private string GenerateRandomString(int length = 4)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            Random random = new Random();
            StringBuilder stringBuilder = new StringBuilder(length);

            for (int i = 0; i < length; i++)
            {
                int index = random.Next(chars.Length);
                stringBuilder.Append(chars[index]);
            }

            return stringBuilder.ToString();
        }

        static int GenerateRandomNumber(int minValue = 1, int maxValue = 1000)
        {
            Random random = new Random();
            return random.Next(minValue, maxValue + 1);
        }

    }
}