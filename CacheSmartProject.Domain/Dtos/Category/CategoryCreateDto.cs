namespace CacheSmartProject.Domain.Dtos.Category;

public class CategoryCreateDto
{
    public string Name { get; set; } = null!;
    public int? ParentId { get; set; }
    
}
