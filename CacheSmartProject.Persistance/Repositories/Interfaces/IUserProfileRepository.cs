using CacheSmartProject.Domain.Entities;

namespace CacheSmartProject.Persistence.Repositories.Interfaces;

public interface IUserProfileRepository
{
    Task AddAsync(UserProfile user);
    Task Update(UserProfile user);
    Task Delete(int id);
    Task<List<UserProfile>> GetAllAsync();   
    Task<UserProfile?> GetByIdAsync(int id);
    Task<DateTime?> GetLastModifiedAsync();
}
