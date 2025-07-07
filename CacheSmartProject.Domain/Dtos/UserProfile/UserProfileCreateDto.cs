namespace CacheSmartProject.Domain.Dtos.UserProfile;

public class UserProfileCreateDto
{
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Preferences { get; set; } = null!;
}
