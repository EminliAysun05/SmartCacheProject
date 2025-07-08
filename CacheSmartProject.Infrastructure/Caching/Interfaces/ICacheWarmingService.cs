namespace CacheSmartProject.Infrastructure.Caching.Interfaces;

public interface ICacheWarmingService
{
    Task WarmUpAsync();
}
