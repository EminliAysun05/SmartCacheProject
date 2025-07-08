using AutoMapper;
using CacheSmartProject.Application.Exceptions;
using CacheSmartProject.Domain.Dtos.Story;
using CacheSmartProject.Domain.Entities;
using CacheSmartProject.Infrastructure.Caching.Interfaces;
using CacheSmartProject.Persistence.Repositories.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using SmartCacheProject.Application.Services.Interfaces;
using StackExchange.Redis;
using System.Text.Json;

namespace CacheSmartProject.Infrastructure.Services;

public class StoryService : IStoryService, ICacheWarmingService
{
    private const string DataKey = "stories:data";
    private const string LastModifiedKey = "stories:lastModified";
    private const string MemoryCacheKey = "stories:memory";

    private readonly IStoryRepository _repository;
    private readonly IDatabase _redisDb;
    private readonly IMemoryCache _memoryCache;
    private readonly IMapper _mapper;
    private readonly ILogger<StoryService> _logger;

    public StoryService(
        IStoryRepository repository,
        IConnectionMultiplexer redis,
        IMemoryCache memoryCache,
        IMapper mapper,
        ILogger<StoryService> logger)
    {
        _repository = repository;
        _redisDb = redis.GetDatabase();
        _memoryCache = memoryCache;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<List<StoryResponseDto>> GetStoriesAlwaysAsync(DateTime? clientLastModified)
    {
        try
        {
            if (_memoryCache.TryGetValue(MemoryCacheKey, out List<StoryResponseDto> memoryStories))
            {
                _logger.LogInformation("Story data loaded from MemoryCache.");
                return memoryStories;
            }

            var redisData = await _redisDb.StringGetAsync(DataKey);
            if (!redisData.IsNullOrEmpty)
            {
                try
                {
                    var redisStories = JsonSerializer.Deserialize<List<StoryResponseDto>>(redisData);
                    if (redisStories is not null)
                    {
                        _logger.LogInformation("Story data loaded from Redis.");
                        _memoryCache.Set(MemoryCacheKey, redisStories, TimeSpan.FromMinutes(5));
                        return redisStories;
                    }
                }
                catch (JsonException ex)
                {
                    _logger.LogError(ex, "Redis deserialization error.");
                }
            }

            var dbStories = await _repository.GetAllAsync();
            var response = _mapper.Map<List<StoryResponseDto>>(dbStories);
            _logger.LogInformation("✔ Story data loaded from Database.");

            await RefreshCacheAsync(response);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while getting stories.");
            throw new ApplicationException("Storilər yüklənərkən xəta baş verdi.");
        }
    }

    public async Task<StoryResponseDto?> GetByIdAsync(int id)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(id);
            return entity == null ? null : _mapper.Map<StoryResponseDto>(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetById əməliyyatı zamanı xəta.");
            throw;
        }
    }

    public async Task AddAsync(StoryCreateDto dto)
    {
        try
        {
            var entity = _mapper.Map<Story>(dto);
            entity.LastModified = DateTime.UtcNow;
            await _repository.AddAsync(entity);

            await InvalidateCacheAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Story əlavə edilərkən xəta baş verdi.");
            throw;
        }
    }

    public async Task UpdateAsync(StoryUpdateDto dto)
    {
        try
        {
            var existing = await _repository.GetByIdAsync(dto.Id);
            if (existing == null)
            {
                _logger.LogWarning("Yenilənəcək story tapılmadı. ID: {Id}", dto.Id);
                throw new NotFoundException($"Story with ID {dto.Id} not found.");
            }

            existing.Title = dto.Title;
            existing.Content = dto.Content;
            existing.ImageUrl = dto.ImageUrl;
            existing.IsPublished = dto.IsPublished;
            existing.LastModified = DateTime.UtcNow;

            await _repository.Update(existing);

            await InvalidateCacheAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Story yenilənərkən xəta baş verdi.");
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
                _logger.LogWarning("Silinəcək story tapılmadı. ID: {Id}", id);
                throw new NotFoundException($"Story with ID {id} not found.");
            }

            await InvalidateCacheAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Story silinərkən xəta baş verdi.");
            throw;
        }
    }

    public async Task<bool> HasStoryChangedAsync(DateTime clientLastModified)
    {
        try
        {
            var redisValue = await _redisDb.StringGetAsync(LastModifiedKey);
            if (redisValue.IsNullOrEmpty) return true;

            var lastModified = DateTime.Parse(redisValue!);
            return lastModified > clientLastModified;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Story dəyişiklik yoxlanışı zamanı xəta.");
            return true;
        }
    }

    private async Task RefreshCacheAsync(List<StoryResponseDto> data)
    {
        try
        {
            await _redisDb.StringSetAsync(DataKey, JsonSerializer.Serialize(data));
            await _redisDb.StringSetAsync(LastModifiedKey, DateTime.UtcNow.ToString("O"));
            _memoryCache.Set(MemoryCacheKey, data, TimeSpan.FromMinutes(5));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Cache refresh zamanı xəta baş verdi.");
        }
    }

    private async Task InvalidateCacheAsync()
    {
        try
        {
            var stories = await _repository.GetAllAsync();
            var response = _mapper.Map<List<StoryResponseDto>>(stories);
            await RefreshCacheAsync(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Cache invalidate zamanı xəta baş verdi.");
        }
    }

    public async Task WarmUpAsync()
    {
        var stories = await _repository.GetAllAsync();
        var dtoList = _mapper.Map<List<StoryResponseDto>>(stories);

        await _redisDb.StringSetAsync("stories:data", JsonSerializer.Serialize(dtoList));
        _memoryCache.Set("stories:memory", dtoList, TimeSpan.FromMinutes(5));
        Console.WriteLine("Story cache warmed.");
    }
}
