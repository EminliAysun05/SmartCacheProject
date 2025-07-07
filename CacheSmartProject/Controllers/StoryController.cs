using CacheSmartProject.Domain.Dtos.Story;
using Microsoft.AspNetCore.Mvc;
using SmartCacheProject.Application.Services.Interfaces;

namespace CacheSmartProject.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StoryController : ControllerBase
{
    private readonly IStoryService _storyService;

    public StoryController(IStoryService storyService)
    {
        _storyService = storyService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _storyService.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var result = await _storyService.GetByIdAsync(id);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] StoryCreateDto dto)
    {
        await _storyService.AddAsync(dto);
        return StatusCode(StatusCodes.Status201Created);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] StoryUpdateDto dto)
    {
        await _storyService.UpdateAsync(dto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _storyService.DeleteAsync(id);
        return NoContent();
    }

  
}
