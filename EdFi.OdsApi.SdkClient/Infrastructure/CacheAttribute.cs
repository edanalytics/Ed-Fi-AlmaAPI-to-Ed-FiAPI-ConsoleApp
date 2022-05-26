using AspectCore.DynamicProxy;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;

namespace EdFi.AlmaToEdFi.Cmd.Infrastructure
{
    public class CacheAttribute : AbstractInterceptorAttribute
    {
        IMemoryCache cache = new MemoryCache(new MemoryCacheOptions());
        public async override Task Invoke(AspectContext context, AspectDelegate next)
        {
            //context.;
            // Only apply cache to the Extractors.
            if (context.Implementation.ToString().Contains(".Extractor")
                && context.ProxyMethod.Name == "Extract")
            {
                var cacheKey = GetCacheKey(context);

                Console.WriteLine($"Caching - {cacheKey}");

                object cachedResult;
                var cacheFound = cache.TryGetValue(cacheKey, out cachedResult);

                if (cacheFound)
                {
                    Console.WriteLine($"Cache found - {cacheKey}");

                    // Do not execute and return the cached resutl.
                    context.ReturnValue = cachedResult;
                }
                else
                {
                    // Actually execute the method.
                    await next(context);

                    // Get the result
                    var result = context.ReturnValue;
                    cache.Set(cacheKey, result);
                }
            }
            else
            {
                // Execute the method.
                await next(context);
            }
        }

        public string GetCacheKey(AspectContext context)
        {
            var Delimiter = "|";
            var NullWithDelimiter = "null|";
            var methodName = context.Implementation.ToString() + "_" + context.ProxyMethod.Name;

            var paramsSent = "(";
            foreach (var p in context.Parameters)
            {
                if (!string.IsNullOrEmpty(p.ToString()))
                {
                    paramsSent += p + ",";
                }
               
            }
            if (paramsSent.EndsWith(","))
            {
                paramsSent = paramsSent.Remove(paramsSent.Length - 1, 1);
            }
            paramsSent += ")";

            return methodName + paramsSent;
        }
    }
}
