using CacheSmartProject.Domain.Entities.Common;

namespace CacheSmartProject.Domain.Entities;

public class Service : BaseEntity
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public DateTime LastModified { get; set; }
    public bool IsActive { get; set; }
}
