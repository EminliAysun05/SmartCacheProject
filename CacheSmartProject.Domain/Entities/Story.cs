using CacheSmartProject.Domain.Entities.Common;

namespace CacheSmartProject.Domain.Entities;

public class Story : BaseEntity
{
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;
    public string? ImageUrl { get; set; }
    public DateTime LastModified { get; set; }
    public bool IsPublished { get; set; }
}
