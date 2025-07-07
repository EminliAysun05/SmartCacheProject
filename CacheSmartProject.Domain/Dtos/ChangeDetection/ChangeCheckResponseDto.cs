namespace CacheSmartProject.Domain.Dtos.ChangeDetection;

public class ChangeCheckResponseDto
{
    public bool CategoriesChanged { get; set; }
    public bool ServicesChanged { get; set; }
    public bool StoriesChanged { get; set; }
}
