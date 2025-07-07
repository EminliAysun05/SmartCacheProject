namespace CacheSmartProject.Domain.Dtos.Story;

public class StoryUpdateDto
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;
    public string? ImageUrl { get; set; }
    public bool IsPublished { get; set; }
}
