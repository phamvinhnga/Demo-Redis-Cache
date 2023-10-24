
using DemoRedisCache.Services;
using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core.Configuration;

namespace DemoRedisCache.Installers
{
    public class RedisInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            var redisConfiguration = new RedisConfiguration();
            configuration.GetSection("RedisConfiguration").Bind(redisConfiguration);

            services.AddSingleton(redisConfiguration);

            if (!string.IsNullOrEmpty(redisConfiguration.ConnectionString))
            {
                _ = services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(configuration: redisConfiguration.ConnectionString));
                services.AddStackExchangeRedisCache(options =>
                {
                    options.Configuration = redisConfiguration.ConnectionString;
                });

                services.AddSingleton<ICacheService, CacheService>();
            }
        }
    }
}
