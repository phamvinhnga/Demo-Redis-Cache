using DemoRedisCache.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net.Mime;
using System.Text;

namespace DemoRedisCache.Attributes
{
    public class CacheAttribute : Attribute, IAsyncActionFilter
    {
        private readonly int _timeToLiveSecounds;

        public CacheAttribute(int timeToLiveSecounds = 100)
        {
            _timeToLiveSecounds = timeToLiveSecounds;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var cacheService = context.HttpContext.RequestServices.GetService<ICacheService>();
            var cacheKey = GennerateCacheKey(context.HttpContext.Request);

            if (!string.IsNullOrWhiteSpace(cacheKey) && cacheService != null)
            {
                var cacheResponse = await cacheService.GetAsync(cacheKey);

                if (!string.IsNullOrEmpty(cacheResponse))
                {
                    var contentResult = new ContentResult
                    {
                        Content = cacheResponse,
                        ContentType = MediaTypeNames.Application.Json,
                        StatusCode = StatusCodes.Status200OK
                    };
                    context.Result = contentResult;
                    return;
                }
            }

            var excutedContext = await next();

            if(!string.IsNullOrWhiteSpace(cacheKey) && cacheService != null && excutedContext.Result is OkObjectResult objectResult && objectResult.Value != null)
            {
                await cacheService.SetAsync(cacheKey, objectResult.Value, TimeSpan.FromSeconds(_timeToLiveSecounds));
            }
        }

        private string GennerateCacheKey(HttpRequest httpRequest)
        {
            var keyBuilder = new StringBuilder();
            keyBuilder.Append(httpRequest.Path);

            foreach (var (key, value) in httpRequest.Query.OrderBy(x => x.Key).ToList())
            {
                keyBuilder.Append($"|{key}-{value}");
            }
            return keyBuilder.ToString();
        }
    }
}
