namespace CacheSmartProject.Domain.Dtos.Service;

public class ServiceResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public DateTime LastModified { get; set; }
    public bool IsActive { get; set; }
}
