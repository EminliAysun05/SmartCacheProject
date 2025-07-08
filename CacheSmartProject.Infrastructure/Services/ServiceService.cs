using AutoMapper;
using CacheSmartProject.Domain.Dtos.Service;
using CacheSmartProject.Domain.Entities;
using CacheSmartProject.Infrastructure.Caching.Interfaces;
using CacheSmartProject.Persistence.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using SmartCacheProject.Application.Services.Interfaces;
using StackExchange.Redis;
using System.Text.Json;

namespace CacheSmartProject.Infrastructure.Services;

public class ServiceService : IServiceService, ICacheWarmingService
{
    private const string DataKey = "services:data";
    private const string LastModifiedKey = "services:lastModified";
    private const string MemoryCacheKey = "services:memory";

    private readonly IServiceRepository _repository;
    private readonly IDatabase _redisDb;
    private readonly IMemoryCache _memoryCache;
    private readonly IMapper _mapper;
    private readonly ILogger<ServiceService> _logger;

    public ServiceService(IServiceRepository repository, IConnectionMultiplexer redis, IMemoryCache memoryCache, IMapper mapper, ILogger<ServiceService> logger)
    {
        _repository = repository;
        _redisDb = redis.GetDatabase();
        _memoryCache = memoryCache;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<List<ServiceResponseDto>> GetServicesAlwaysAsync(DateTime? clientLastModified)
    {
        try
        {
            if (_memoryCache.TryGetValue("services", out List<Service> cachedServices))
            {
                Console.WriteLine("Data loaded from MemoryCache");
                return _mapper.Map<List<ServiceResponseDto>>(cachedServices);
            }

            var redisData = await _redisDb.StringGetAsync("services");
            if (!redisData.IsNullOrEmpty)
            {
                try
                {
                    var redisServices = JsonSerializer.Deserialize<List<Service>>(redisData);
                    if (redisServices is not null)
                    {
                        Console.WriteLine("Data loaded from Redis");
                        _memoryCache.Set("services", redisServices, TimeSpan.FromMinutes(5));
                        return _mapper.Map<List<ServiceResponseDto>>(redisServices);
                    }
                }
                catch (JsonException ex)
                {
                    _logger.LogError(ex, "Redis-dən deserializasiya xətası.");
                }
            }

            var dbServices = await _repository.GetAllAsync();
            Console.WriteLine("Data loaded from Database");

            var serialized = JsonSerializer.Serialize(dbServices);
            await _redisDb.StringSetAsync("services", serialized);
            _memoryCache.Set("services", dbServices, TimeSpan.FromMinutes(5));

            return _mapper.Map<List<ServiceResponseDto>>(dbServices);
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex, "Verilənlər bazası ilə əlaqə zamanı xəta.");
            throw new ApplicationException("Xidmətləri əldə etmək mümkün olmadı. Zəhmət olmasa, sonra yenidən cəhd edin.");
        }
        catch (RedisConnectionException ex)
        {
            _logger.LogError(ex, "Redis bağlantısı uğursuz oldu.");
            throw new ApplicationException("Keş sisteminə bağlantı qurmaq mümkün olmadı.");
        }
        catch (AutoMapperMappingException ex)
        {
            _logger.LogError(ex, "AutoMapper xəta verdi.");
            throw new ApplicationException("Verilənlərə uyğun çevrilmə zamanı xəta baş verdi.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Gözlənilməz xəta baş verdi.");
            throw new ApplicationException("Naməlum bir xəta baş verdi.");
        }
    }

    public async Task AddAsync(ServiceCreateDto dto)
    {
        try
        {
            var entity = _mapper.Map<Service>(dto);
            entity.LastModified = DateTime.UtcNow;
            await _repository.AddAsync(entity);
            await InvalidateCache();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Service əlavə edilərkən xəta baş verdi.");
            throw;
        }
    }

    public async Task DeleteAsync(int id)
    {
        try
        {
            bool deleted = await _repository.Delete(id);
            if (!deleted)
            {
                _logger.LogWarning("ID {Id} üçün servis tapılmadı.", id);
                throw new KeyNotFoundException($"Service with ID {id} not found.");
            }

            await InvalidateCache();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Service silinərkən xəta baş verdi.");
            throw;
        }
    }


    private async Task InvalidateCache()
    {
       try
        {
            List<Service> updatedData = await _repository.GetAllAsync();
            await RefreshCache(updatedData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Keş yenilənərkən xəta baş verdi.");
        }
    }

    private async Task RefreshCache(List<Service> data)
    {
        try
        {
            string json = JsonSerializer.Serialize(data);
            await _redisDb.StringSetAsync(DataKey, json);
            await _redisDb.StringSetAsync(LastModifiedKey, DateTime.UtcNow.ToString("O"));
            _memoryCache.Set(MemoryCacheKey, data, TimeSpan.FromMinutes(10));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Keş yazılarkən xəta baş verdi.");
        }
    }
    public async Task<DateTime?> GetLastModifiedAsync()
    {
        try
        {
            return await _repository.GetLastModifiedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Son dəyişiklik tarixi alınarkən xəta.");
            return null;
        }
    }

    public async Task<bool> HasServiceChanged(DateTime clientLastModified)
    {
        try
        {
            var dbLastModified = await _repository.GetLastModifiedAsync();
            return dbLastModified != null && dbLastModified > clientLastModified;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Xidmət dəyişiklik yoxlanışı zamanı xəta.");
            return true; 
        }
    }

    public async Task<bool> UpdateAsync(ServiceUpdateDto dto)
    {
        try
        {
            var entity = _mapper.Map<Service>(dto);
            entity.LastModified = DateTime.UtcNow;

            var updated = await _repository.Update(entity);

            if (updated)
            {
                await InvalidateCache(); 
            }
            else
            {
                _logger.LogWarning("Yenilənəcək Service tapılmadı. ID: {Id}", entity.Id);
            }

            return updated;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Service yenilənərkən xəta baş verdi.");
            throw;
        }
    }

    public async Task WarmUpAsync()
    {
        var services = await _repository.GetAllAsync();
        var response = _mapper.Map<List<ServiceResponseDto>>(services);

        await _redisDb.StringSetAsync("services:data", JsonSerializer.Serialize(response));
        await _redisDb.StringSetAsync("services:lastModified", DateTime.UtcNow.ToString("O"));
        _memoryCache.Set("services:memory", response, TimeSpan.FromMinutes(5));

        Console.WriteLine("Service cache warmed.");
    }
}
