using DemoRedisCache.Context;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace DemoRedisCache.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ListController : ControllerBase
    {
        private readonly IDatabase _redisDatabase;

        public ListController(
            IRedisConnectionMultiplexer redisConnectionMultiplexer)
        {
            _redisDatabase = redisConnectionMultiplexer.GetDatabase();
        }

        [HttpGet("rpush")]
        public async Task<IActionResult> RightpushAsync()
        {
            var key = new RedisKey("temps");

            var values = new string[] { "A", "B", "C" };
            var redisValues = Array.ConvertAll(values, item => (RedisValue)item);
      
            var result = await _redisDatabase.ListRightPushAsync(key, redisValues);
            return Ok(result);
        }

        [HttpGet("lpush")]
        public async Task<IActionResult> LeftpushAsync()
        {
            var key = new RedisKey("temps");

            var values = new string[] { "D", "E", "F" };
            var redisValues = Array.ConvertAll(values, item => (RedisValue)item);

            var result = await _redisDatabase.ListLeftPushAsync(key, redisValues);
            return Ok(result);
        }
    }
}
    