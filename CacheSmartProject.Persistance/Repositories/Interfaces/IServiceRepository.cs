using CacheSmartProject.Domain.Entities;


namespace CacheSmartProject.Persistence.Repositories.Interfaces;

public interface IServiceRepository
{
    Task<List<Service>> GetAllAsync();
    Task<Service?> GetByIdAsync(int id);
    Task AddAsync(Service service);
    Task Update(Service service);
    Task<bool> Delete(int id);
    Task<DateTime?> GetLastModifiedAsync();
    
}
