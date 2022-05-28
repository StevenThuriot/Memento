using Grpc.Core;
using Microsoft.Extensions.Caching.Memory;

namespace Memento.Services;

public class MementoService : Cache.CacheBase
{
    private readonly IMemoryCache _cache;

    public MementoService(IMemoryCache cache)
    {
        _cache = cache;
    }

    public override Task<Result> GetAsync(GetRequest request, ServerCallContext context)
    {
        if (_cache.TryGetValue(request.Key, out byte[] result))
        {
            return Result.FromAsync(result);
        }
        else
        {
            return Result.NotFound;
        }
    }

    public override Task<Result> Invalidate(InvalidateRequest request, ServerCallContext context)
    {
        _cache.Remove(request.Key);
        return Result.Success;
    }

    public override Task<Result> SetAsync(SetRequest request, ServerCallContext context)
    {
        if (request.Value is null)
        {
            return FailOnNull();
        }

        _ = SetInternal(request);

        return Result.Success;
    }

    public override Task<Result> GetOrCreateAsync(SetRequest request, ServerCallContext context)
    {
        if (!_cache.TryGetValue(request.Key, out var value))
        {
            if (request.Value is null)
            {
                return FailOnNull();
            }

            return Result.FromAsync(SetInternal(request));
        }

        return Result.FromAsync((byte[]) value);
    }

    private static Task<Result> FailOnNull() => Result.FailAsync("Unable to cache empty value");

    private byte[] SetInternal(SetRequest request)
    {
        var result = request.Value.ToByteArray();

        using var entry = _cache.CreateEntry(request.Key)
                                .SetValue(result);

        var timespan = request.Expiration?.ToTimeSpan();

        if (timespan.HasValue)
        {
            if (request.Sliding)
            {
                entry.SetSlidingExpiration(timespan.GetValueOrDefault());
            }
            else
            {
                entry.SetAbsoluteExpiration(timespan.GetValueOrDefault());
            }
        }

        return result;
    }
}