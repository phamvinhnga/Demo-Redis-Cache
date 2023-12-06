using DemoRedisCache.Context;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace DemoRedisCache.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SortedSetController : ControllerBase
    {
        private readonly IDatabase _redisDatabase;

        public SortedSetController(
            IRedisConnectionMultiplexer redisConnectionMultiplexer)
        {
            _redisDatabase = redisConnectionMultiplexer.GetDatabase();
        }

        [HttpGet("zmscore")]
        public async Task<IActionResult> SortedSetScoresAsync()
        {
            var result = await _redisDatabase.SortedSetScoresAsync("followers", members: new RedisValue[]
            {
                new RedisValue("1"),
                new RedisValue("6")
            });
            return Ok(result);
        }

        [HttpGet("zpopmax")]
        public async Task<IActionResult> SortedSetPopDescending()
        {
            var result = await _redisDatabase.SortedSetPopAsync(
                key: "followers",
                count: 2,
                order: Order.Descending);

            return Ok(result);
        }

        [HttpGet("zpopmin")]
        public async Task<IActionResult> SortedSetPopAscending()
        {
            var result = await _redisDatabase.SortedSetPopAsync(
                key: "followers", 
                count: 10, 
                order: Order.Ascending);

            return Ok(result);
        }

        [HttpGet("zcard")]
        public async Task<IActionResult> FindingRangeOfScores()
        {
            var result = await _redisDatabase.SortedSetLengthAsync("followers", 10, 100);
            return Ok(result);
        }

        [HttpPost("zadd/{i}")]
        public async Task<IActionResult> Post(int i)
        {
            var result = await _redisDatabase.SortedSetAddAsync("followers", $"{i}", GenerateRandomNumber());
            return Ok(result);
        }

        private double GenerateRandomNumber(int minValue = 1, int maxValue = 1000)
        {
            Random random = new Random();
            return (double)random.Next(minValue, maxValue + 1);
        }


    }
}