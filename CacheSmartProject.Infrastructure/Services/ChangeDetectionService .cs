
using CacheSmartProject.Domain.Dtos.ChangeDetection;
using CacheSmartProject.Infrastructure.Caching.Interfaces;
using SmartCacheProject.Application.Services.Interfaces;

namespace CacheSmartProject.Infrastructure.Services;

public class ChangeDetectionService : IChangeDetectionService
{
    private readonly IRedisCacheService _redis;

    public ChangeDetectionService(IRedisCacheService redis)
    {
        _redis = redis;
    }

    public async Task<ChangeCheckResponseDto> CheckChangesAsync(ChangeCheckRequestDto request)
    {
        var categoriesLast = await _redis.GetLastModifiedAsync("categories");
        var servicesLast = await _redis.GetLastModifiedAsync("services");
        var storiesLast = await _redis.GetLastModifiedAsync("stories");

        return new ChangeCheckResponseDto
        {
            CategoriesChanged = categoriesLast > request.CategoriesLastFetched,
            ServicesChanged = servicesLast > request.ServicesLastFetched,
            StoriesChanged = storiesLast > request.StoriesLastFetched
        };
    }
}
