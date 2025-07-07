namespace CacheSmartProject.Domain.Dtos.Story;

public class StoryResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;
    public string ImageUrl { get; set; } = null!;
    public DateTime LastModified { get; set; }
    public bool IsPublished { get; set; }
}