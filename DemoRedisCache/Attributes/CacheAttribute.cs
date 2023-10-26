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
        private ICacheService _cacheService;

        public CacheAttribute(int timeToLiveSecounds = 100)
        {
            _timeToLiveSecounds = timeToLiveSecounds;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            _cacheService = context.HttpContext.RequestServices.GetService<ICacheService>();

            var cacheKey = GennerateCacheKey(context.HttpContext.Request);

            var isSuccess = await GetCacheResponseAsync(cacheKey, context);
            if(!isSuccess)
            {
                var excutedContext = await next();
                await SetCacheResponseAsync(cacheKey, excutedContext);
            }
        }

        private async Task<bool> GetCacheResponseAsync(string cacheKey, ActionExecutingContext context)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(cacheKey) && _cacheService != null)
                {
                    var cacheResponse = await _cacheService.GetAsync(cacheKey);

                    if (!string.IsNullOrEmpty(cacheResponse))
                    {
                        var contentResult = new ContentResult
                        {
                            Content = cacheResponse,
                            ContentType = MediaTypeNames.Application.Json,
                            StatusCode = StatusCodes.Status200OK
                        };
                        context.Result = contentResult;
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return false;
        }

        private async Task SetCacheResponseAsync(string cacheKey, ActionExecutedContext? excutedContext)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(cacheKey) && _cacheService != null && excutedContext != null && excutedContext.Result is OkObjectResult objectResult && objectResult.Value != null)
                {
                    await _cacheService.SetAsync(cacheKey, objectResult.Value, TimeSpan.FromSeconds(_timeToLiveSecounds));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
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
