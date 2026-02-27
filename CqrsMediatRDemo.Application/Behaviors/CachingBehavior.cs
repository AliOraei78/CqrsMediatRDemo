using MediatR;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace CqrsMediatRDemo.Application.Behaviors;

public sealed class CachingBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IMemoryCache _cache;
    private readonly MemoryCacheEntryOptions _cacheOptions;

    public CachingBehavior(IMemoryCache cache)
    {
        _cache = cache;
        _cacheOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromMinutes(5))
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(30));
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (typeof(TResponse) == typeof(Unit))
        {
            return await next();
        }

        var cacheKey = GenerateCacheKey(request);

        if (_cache.TryGetValue(cacheKey, out TResponse? cachedResponse) && cachedResponse is not null)
        {
            Console.WriteLine($"---> Cache HIT for key: {cacheKey}");
            return cachedResponse;
        }
        Console.WriteLine($"---> Cache MISS for key: {cacheKey}. Fetching from source...");
        var response = await next();

        _cache.Set(cacheKey, response, _cacheOptions);

        return response;
    }

    private static string GenerateCacheKey(TRequest request)
    {
        var type = typeof(TRequest);
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        var values = properties
            .Select(p => $"{p.Name}:{p.GetValue(request)}")
            .Aggregate((a, b) => $"{a}|{b}");

        return $"{type.Name}:{values}";
    }
}
