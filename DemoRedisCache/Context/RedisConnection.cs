using StackExchange.Redis;

namespace DemoRedisCache.Context
{
    public interface IRedisConnectionMultiplexer
    {
        ConnectionMultiplexer ConnectionMultiplexer { get; }
        IDatabase GetDatabase();
    }

    public class RedisConnectionMultiplexer : IRedisConnectionMultiplexer
    {
        public ConnectionMultiplexer ConnectionMultiplexer
        {
            get
            {
                return StackExchange.Redis.ConnectionMultiplexer.Connect($"redis-10534.c100.us-east-1-4.ec2.cloud.redislabs.com:10534,password=DEVkYmqovqxOJXOHq77X7AiSUhl3vOa8");
            }
        }

        public IDatabase GetDatabase()
        {
            return ConnectionMultiplexer.GetDatabase();
        }
    }
}
