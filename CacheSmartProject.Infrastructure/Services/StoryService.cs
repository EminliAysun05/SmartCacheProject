using AutoMapper;
using CacheSmartProject.Domain.Dtos.Story;
using CacheSmartProject.Domain.Entities;
using CacheSmartProject.Persistence.Repositories.Implementations;
using CacheSmartProject.Persistence.Repositories.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using SmartCacheProject.Application.Services.Interfaces;
using StackExchange.Redis;
using System.Text.Json;

namespace CacheSmartProject.Infrastructure.Services;
public class StoryService : IStoryService
{
    private const string DataKey = "stories:data";
    private const string LastModifiedKey = "stories:lastModified";
    private const string MemoryCacheKey = "stories:memory";

    private readonly IStoryRepository _repository;
    private readonly IDatabase _redisDb;
    private readonly IMemoryCache _memoryCache;
    private readonly IMapper _mapper;

    public StoryService(
        IStoryRepository repository,
        IConnectionMultiplexer redis,
        IMemoryCache memoryCache,
        IMapper mapper)
    {
        _repository = repository;
        _redisDb = redis.GetDatabase();
        _memoryCache = memoryCache;
        _mapper = mapper;
    }

    public async Task<List<StoryResponseDto>> GetAllAsync()
    {
        if (_memoryCache.TryGetValue(MemoryCacheKey, out List<StoryResponseDto> cached))
            return cached;

        var redisValue = await _redisDb.StringGetAsync(DataKey);
        if (!redisValue.IsNullOrEmpty)
        {
            var data = JsonSerializer.Deserialize<List<StoryResponseDto>>(redisValue!);
            _memoryCache.Set(MemoryCacheKey, data!, TimeSpan.FromMinutes(5));
            return data!;
        }

        var stories = await _repository.GetAllAsync();
        var response = _mapper.Map<List<StoryResponseDto>>(stories);

        await _redisDb.StringSetAsync(DataKey, JsonSerializer.Serialize(response));
        await _redisDb.StringSetAsync(LastModifiedKey, (await _repository.GetLastModifiedAsync())?.ToString("O"));
        _memoryCache.Set(MemoryCacheKey, response, TimeSpan.FromMinutes(5));

        return response;
    }

    public async Task<StoryResponseDto?> GetByIdAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        return entity == null ? null : _mapper.Map<StoryResponseDto>(entity);
    }

    public async Task AddAsync(StoryCreateDto dto)
    {
        var entity = _mapper.Map<Story>(dto);
        entity.LastModified = DateTime.UtcNow;
        await _repository.AddAsync(entity);

        await RefreshCacheAsync();
    }

    public async Task UpdateAsync(StoryUpdateDto dto)
    {
        var existing = await _repository.GetByIdAsync(dto.Id);
        if (existing == null) return;

        existing.Title = dto.Title;
        existing.Content = dto.Content;
        existing.ImageUrl = dto.ImageUrl;
        existing.IsPublished = dto.IsPublished;
        existing.LastModified = DateTime.UtcNow;

        await _repository.Update(existing);

        await RefreshCacheAsync();
    }

    public async Task DeleteAsync(int id)
    {
        await _repository.Delete(id);
        await RefreshCacheAsync();
    }

    public async Task<bool> HasStoryChangedAsync(DateTime clientLastModified)
    {
        var redisValue = await _redisDb.StringGetAsync(LastModifiedKey);
        if (redisValue.IsNullOrEmpty) return true;

        var lastModified = DateTime.Parse(redisValue!);
        return lastModified > clientLastModified;
    }

    private async Task RefreshCacheAsync()
    {
        var stories = await _repository.GetAllAsync();
        var response = _mapper.Map<List<StoryResponseDto>>(stories);

        await _redisDb.StringSetAsync(DataKey, JsonSerializer.Serialize(response));
        await _redisDb.StringSetAsync(LastModifiedKey, DateTime.UtcNow.ToString("O"));
        _memoryCache.Set(MemoryCacheKey, response, TimeSpan.FromMinutes(5));
    }
}
