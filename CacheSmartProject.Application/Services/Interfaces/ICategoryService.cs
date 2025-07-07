using CacheSmartProject.Domain.Dtos.Category;

namespace CacheSmartProject.Application.Services.Interfaces;

public interface ICategoryService
{
    Task<List<CategoryResponseDto>> GetCategoriesAlwaysAsync(DateTime? clientLastModified);
    Task AddAsync(CategoryCreateDto dto);
    Task UpdateAsync(CategoryUpdateDto dto);
    Task DeleteAsync(int id);
    Task<DateTime?> GetLastModifiedAsync();
    Task<bool> HasCategoryChanged(DateTime clientLastModified);
}
