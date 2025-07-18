﻿using AutoMapper;
using CacheSmartProject.Application.Exceptions;
using CacheSmartProject.Application.Services.Interfaces;
using CacheSmartProject.Domain.Dtos.Category;
using CacheSmartProject.Domain.Entities;
using CacheSmartProject.Infrastructure.Caching.Interfaces;
using CacheSmartProject.Persistence.Repositories.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Text.Json;

namespace CacheSmartProject.Infrastructure.Services;

public class CategoryService : ICategoryService, ICacheWarmingService
{
    private const string DataKey = "categories:data";
    private const string LastModifiedKey = "categories:lastModified";
    private const string MemoryCacheKey = "categories:memory";

    private readonly ICategoryRepository _repository;
    private readonly IMemoryCache _memoryCache;
    private readonly IDatabase _redisDb;
    private readonly IMapper _mapper;
    private readonly ILogger<CategoryService> _logger;

    public CategoryService(
        ICategoryRepository repository,
        IMemoryCache memoryCache,
        IConnectionMultiplexer redis,
        IMapper mapper,
        ILogger<CategoryService> logger)
    {
        _repository = repository;
        _memoryCache = memoryCache;
        _redisDb = redis.GetDatabase();
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<List<CategoryResponseDto>> GetCategoriesAlwaysAsync(DateTime? clientLastModified)
    {
        try
        {
            if (_memoryCache.TryGetValue(MemoryCacheKey, out List<Category> memoryData))
            {
                _logger.LogInformation("Data loaded from MemoryCache");
                return _mapper.Map<List<CategoryResponseDto>>(memoryData);
            }

            var redisData = await _redisDb.StringGetAsync(DataKey);
            if (!redisData.IsNullOrEmpty)
            {
                var redisCategories = JsonSerializer.Deserialize<List<Category>>(redisData);
                if (redisCategories is not null)
                {
                    _logger.LogInformation("Data loaded from Redis");
                    _memoryCache.Set(MemoryCacheKey, redisCategories, TimeSpan.FromMinutes(5));
                    return _mapper.Map<List<CategoryResponseDto>>(redisCategories);
                }
            }

            var dbCategories = await _repository.GetAllAsync();
            _logger.LogInformation("Data loaded from Database");

            string json = JsonSerializer.Serialize(dbCategories);
            await _redisDb.StringSetAsync(DataKey, json);
            await _redisDb.StringSetAsync(LastModifiedKey, DateTime.UtcNow.ToString("O"));
            _memoryCache.Set(MemoryCacheKey, dbCategories, TimeSpan.FromMinutes(5));

            return _mapper.Map<List<CategoryResponseDto>>(dbCategories);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Category data loading failed.");
            throw;
        }
    }

    public async Task AddAsync(CategoryCreateDto dto)
    {
        try
        {
            if(dto.ParentId.HasValue)
        {
                var parent = await _repository.GetByIdAsync(dto.ParentId.Value);
                if (parent == null)
                {
                    _logger.LogWarning("Mövcud olmayan ParentId ilə category əlavə edilməyə çalışıldı. ParentId: {ParentId}", dto.ParentId);
                    throw new ValidationException($"Parent category with ID {dto.ParentId} does not exist.");
                }
            }

            var category = _mapper.Map<Category>(dto);
            category.LastModified = DateTime.UtcNow;
            await _repository.AddAsync(category);
            await InvalidateCache();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Category əlavə edilərkən xəta baş verdi.");
            throw;
        }
    }

    public async Task UpdateAsync(CategoryUpdateDto dto)
    {
        try
        {
            var existing = await _repository.GetByIdAsync(dto.Id);
            if (existing == null)
            {
                _logger.LogWarning("Yenilənəcək category tapılmadı. ID: {Id}", dto.Id);
                throw new NotFoundException($"Category with ID {dto.Id} not found.");
            }

            if (dto.ParentId.HasValue)
            {
                var parent = await _repository.GetByIdAsync(dto.ParentId.Value);
                if (parent == null)
                {
                    _logger.LogWarning("Yeniləmə zamanı mövcud olmayan ParentId göstərildi. ParentId: {ParentId}", dto.ParentId);
                    throw new ValidationException($"Parent category with ID {dto.ParentId} does not exist.");
                }
            }

            var category = _mapper.Map<Category>(dto);
            category.LastModified = DateTime.UtcNow;
            bool updated = await _repository.Update(category);
            if (!updated)
            {
                _logger.LogWarning("Yenilənəcək category tapılmadı. ID: {Id}", category.Id);
                throw new NotFoundException($"Category with ID {category.Id} not found.");
            }

            await InvalidateCache();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Category yenilənərkən xəta baş verdi.");
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
                _logger.LogWarning("Silinəcək category tapılmadı. ID: {Id}", id);
                throw new NotFoundException($"Category with ID {id} not found.");
            }

            await InvalidateCache();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Category silinərkən xəta baş verdi.");
            throw;
        }
    }

    private async Task InvalidateCache()
    {
        var updatedData = await _repository.GetAllAsync();
        string json = JsonSerializer.Serialize(updatedData);
        await _redisDb.StringSetAsync(DataKey, json);
        await _redisDb.StringSetAsync(LastModifiedKey, DateTime.UtcNow.ToString("O"));
        _memoryCache.Set(MemoryCacheKey, updatedData, TimeSpan.FromMinutes(5));
    }

    public async Task<DateTime?> GetLastModifiedAsync()
    {
        return await _repository.GetLastModifiedAsync();
    }

    public async Task<bool> HasCategoryChanged(DateTime clientLastModified)
    {
        var dbLastModified = await _repository.GetLastModifiedAsync();
        return dbLastModified != null && dbLastModified > clientLastModified;
    }

    public async Task WarmUpAsync()
    {
        var categories = await _repository.GetAllAsync();
        var dtoList = _mapper.Map<List<CategoryResponseDto>>(categories);

        await _redisDb.StringSetAsync("categories:data", JsonSerializer.Serialize(dtoList));
        _memoryCache.Set("categories:memory", dtoList, TimeSpan.FromMinutes(5));
        Console.WriteLine("Category cache warmed.");
    }
}
