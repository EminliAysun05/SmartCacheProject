

using CacheSmartProject.Domain.Dtos.Service;

namespace SmartCacheProject.Application.Services.Interfaces;

public interface IServiceService
{
    Task<List<ServiceResponseDto>> GetServicesAlwaysAsync(DateTime? clientLastModified);
    Task AddAsync(ServiceCreateDto dto);
    Task<bool> UpdateAsync(ServiceUpdateDto dto);
    Task DeleteAsync(int id);
    Task<DateTime?> GetLastModifiedAsync();
    Task<bool> HasServiceChanged(DateTime clientLastModified);

}
