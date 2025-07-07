namespace CacheSmartProject.Domain.Dtos.Category;

public class CategoryCreateDto
{
    public string Name { get; set; } = null!;
    public int? ParentId { get; set; }
    
}

public class CategoryUpdateDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public int? ParentId { get; set; }
    public bool IsActive { get; set; }
}