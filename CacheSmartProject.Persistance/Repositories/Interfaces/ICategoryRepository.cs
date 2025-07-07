

using CacheSmartProject.Domain.Entities;

namespace CacheSmartProject.Persistence.Repositories.Interfaces
{
    public interface ICategoryRepository
    {
        Task<List<Category?>> GetAllAsync();
        Task<Category?> GetByIdAsync(int id);
        Task AddAsync(Category category);
        Task<bool> Update(Category category);
        Task<bool> Delete(int id);
        Task<DateTime?> GetLastModifiedAsync();
    }
}
