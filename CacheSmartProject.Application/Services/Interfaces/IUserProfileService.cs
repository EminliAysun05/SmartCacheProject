

using CacheSmartProject.Domain.Dtos.UserProfile;

namespace SmartCacheProject.Application.Services.Interfaces;

public interface IUserProfileService
{
    Task<List<UserProfileResponseDto>> GetAllAsync();
    Task<UserProfileResponseDto?> GetByIdAsync(int id);
    Task<int> CreateAsync(UserProfileCreateDto dto);
    Task<bool> UpdateAsync(UserProfileUpdateDto dto);
    Task<bool> DeleteAsync(int id);
}
