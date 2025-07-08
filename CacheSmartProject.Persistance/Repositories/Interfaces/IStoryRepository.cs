using CacheSmartProject.Domain.Entities;


namespace CacheSmartProject.Persistence.Repositories.Interfaces;

public interface IStoryRepository
{
    Task<List<Story>> GetAllAsync();
    Task<Story?> GetByIdAsync(int id);
    Task AddAsync(Story story);
    Task Update(Story story);
    Task<bool> Delete(int id);
    Task<DateTime?> GetLastModifiedAsync();
}
