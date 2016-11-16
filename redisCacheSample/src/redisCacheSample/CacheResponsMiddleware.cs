using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace redisCacheSample
{
    public class CacheResponsMiddleware
    {
        private readonly RequestDelegate next;
        private readonly IDistributedCache cache;
        public CacheResponsMiddleware(RequestDelegate next, IDistributedCache cache)
        {
            this.next = next;
            this.cache = cache;
        }

        public async Task Invoke(HttpContext context)
        {
            string cacheValue = "Nothing Found in Cache";
            var val = await cache.GetAsync("CacheKey");
            if (val != null)
                cacheValue = Encoding.UTF8.GetString(val);

            context.Response.Headers.Append("CacheValue", cacheValue);
            await context.Response.WriteAsync(cacheValue + Environment.NewLine);
            await next.Invoke(context);
        }
    }

    public static class CacheResponseMiddlewareExtension
    {
        public static IApplicationBuilder UseCacheResponse(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CacheResponsMiddleware>();
        }
    }
}
