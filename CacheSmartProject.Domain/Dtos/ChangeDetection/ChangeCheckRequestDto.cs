namespace CacheSmartProject.Domain.Dtos.ChangeDetection;

public class ChangeCheckRequestDto
{
    public DateTime? CategoriesLastFetched { get; set; }
    public DateTime? ServicesLastFetched { get; set; }
    public DateTime? StoriesLastFetched { get; set; }
}
