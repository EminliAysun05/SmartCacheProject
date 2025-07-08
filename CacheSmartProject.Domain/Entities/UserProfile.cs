using CacheSmartProject.Domain.Entities.Common;

namespace CacheSmartProject.Domain.Entities;

public class UserProfile : BaseEntity
{
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Preferences { get; set; } = "{}";
    public DateTime LastModified { get; set; }
}
