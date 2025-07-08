

using CacheSmartProject.Domain.Dtos.Service;
using CacheSmartProject.Domain.Dtos.Story;
using System.Threading.Tasks;

namespace SmartCacheProject.Application.Services.Interfaces;

public interface IStoryService
{
    Task<List<StoryResponseDto>> GetStoriesAlwaysAsync(DateTime? clientLastModified);
    Task<StoryResponseDto?> GetByIdAsync(int id);
    Task AddAsync(StoryCreateDto dto);
    Task UpdateAsync(StoryUpdateDto dto);
    Task DeleteAsync(int id);
    Task<bool> HasStoryChangedAsync(DateTime clientLastModified);

}
