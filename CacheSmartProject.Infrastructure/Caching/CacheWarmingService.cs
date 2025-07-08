using CacheSmartProject.Infrastructure.Caching.Interfaces;

namespace CacheSmartProject.Infrastructure.Caching;

public class CacheWarmingService 
{
    private readonly IEnumerable<ICacheWarmingService> _services;

    public CacheWarmingService(IEnumerable<ICacheWarmingService> services)
    {
        _services = services;
    }

    public async Task WarmAllAsync()
    {
        foreach (var service in _services)
        {
            await service.WarmUpAsync();
        }
    }
}
