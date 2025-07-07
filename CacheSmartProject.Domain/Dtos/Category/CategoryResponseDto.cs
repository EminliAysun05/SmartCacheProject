namespace CacheSmartProject.Domain.Dtos.Category;

public class CategoryResponseDto 
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public int? ParentId { get; set; }
    public DateTime LastModified { get; set; }
    public bool IsActive { get; set; }
}