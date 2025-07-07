namespace CacheSmartProject.Domain.Dtos.UserProfile;

public class UserProfileResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Preferences { get; set; } = null!;
    public DateTime LastModified { get; set; }
}
