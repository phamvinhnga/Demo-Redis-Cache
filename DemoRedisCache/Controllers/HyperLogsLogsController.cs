using DemoRedisCache.Context;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace DemoRedisCache.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HyperLogsLogsController : ControllerBase
    {
        private readonly IDatabase _redisDatabase;

        public HyperLogsLogsController(
            IRedisConnectionMultiplexer redisConnectionMultiplexer)
        {
            _redisDatabase = redisConnectionMultiplexer.GetDatabase();
        }

        //public async Task<IActionResult> SortedSetPopAscending()
        //{
        //    var result = await _redisDatabase.SortedSetPopAsync(
        //        key: "followers",
        //        count: 10,
        //        order: Order.Ascending);

        //    return Ok(result);
        //}
    }
}
