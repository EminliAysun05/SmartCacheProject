using CacheSmartProject.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using SmartCacheProject.Application.Services.Interfaces;


namespace CacheSmartProject.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ChangeDetectionController : ControllerBase
{
    private readonly ICategoryService _categoryService;
    private readonly IServiceService _serviceService;
    private readonly IStoryService _storyService;

    public ChangeDetectionController(ICategoryService categoryService, IServiceService serviceService, IStoryService storyService)
    {
        _categoryService = categoryService;
        _serviceService = serviceService;
        _storyService = storyService;
    }

    [HttpGet]
    public async Task<IActionResult> CheckChanges(
        [FromQuery] DateTime categoriesLastFetched,
        [FromQuery] DateTime servicesLastFetched,
        [FromQuery] DateTime storiesLastFetched)
    {
        var categoryChanged = await _categoryService.HasCategoryChanged(categoriesLastFetched);
        var serviceChanged = await _serviceService.HasServiceChanged(servicesLastFetched);
        var storyChanged = await _storyService.HasStoryChangedAsync(storiesLastFetched);

        var result = new
        {
            CategoryChanged = categoryChanged,
            ServiceChanged = serviceChanged,
            StoryChanged = storyChanged
        };

        return Ok(result);
    }
}
