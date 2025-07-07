using CacheSmartProject.Domain.Dtos.ChangeDetection;


namespace SmartCacheProject.Application.Services.Interfaces;

public interface IChangeDetectionService
{
    Task<ChangeCheckResponseDto> CheckChangesAsync(ChangeCheckRequestDto request);
}
