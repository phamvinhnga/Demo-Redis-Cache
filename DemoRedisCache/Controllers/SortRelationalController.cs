using DemoRedisCache.Context;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Net;

namespace DemoRedisCache.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SortRelationalController : ControllerBase
    {
        private readonly IDatabase _redisDatabase;

        public SortRelationalController(
            IRedisConnectionMultiplexer redisConnectionMultiplexer)
        {
            _redisDatabase = redisConnectionMultiplexer.GetDatabase();
        }

        [HttpGet("init")]
        public async Task<IActionResult> InitAsync()
        {
            await _redisDatabase.HashSetAsync(
                new RedisKey("book:good"),
                new HashEntry[]
                {
                    new HashEntry("title", "Good book"),
                    new HashEntry("year", 1950)
                }
            );
            await _redisDatabase.HashSetAsync(
                new RedisKey("book:bad"),
                new HashEntry[]
                {
                    new HashEntry("title", "Bad book"),
                    new HashEntry("year", 1930)
                }
            );
            await _redisDatabase.HashSetAsync(
                new RedisKey("book:ok"),
                new HashEntry[]
                {
                    new HashEntry("title", "Ok book"),
                    new HashEntry("year", 12)
                }
            );

            await _redisDatabase.SortedSetAddAsync("book:likes", "good", 999);
            await _redisDatabase.SortedSetAddAsync("book:likes", "bad", 0);
            await _redisDatabase.SortedSetAddAsync("book:likes", "ok", 40);

            return Ok();
        }

        [HttpGet("sort")]
        public async Task<IActionResult> SortAsync()
        {
            var result = await _redisDatabase.SortAsync("book:likes", sortType: SortType.Alphabetic);
            return Ok(result.Select(s => s.ToString()).ToList());
        }

        [HttpGet("sort-by")]
        public async Task<IActionResult> SortByAsync()
        {
            var sortedBooks = await _redisDatabase.SortAsync(
                key: new RedisKey("book:likes"),
                by: new RedisValue("book:*->year"),
                get: new RedisValue[]
                {
                    "book:*->title",
                    "book:*->year",
                },
                flags: CommandFlags.None,
                sortType: SortType.Alphabetic);

            return Ok(sortedBooks.Select(s => s.ToString()).ToList());
        }
    }
}
