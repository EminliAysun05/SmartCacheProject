using CacheSmartProject.Domain.Dtos.Service;
using Microsoft.AspNetCore.Mvc;
using SmartCacheProject.Application.Services.Interfaces;


namespace CacheSmartProject.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ServiceController : ControllerBase
{
    private readonly IServiceService _serviceService;

    public ServiceController(IServiceService serviceService)
    {
        _serviceService = serviceService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] DateTime? lastModified)
    {
        var services = await _serviceService.GetServicesAlwaysAsync(lastModified);
        return Ok(services);
    }

    [HttpPost]
    public async Task<IActionResult> Create(ServiceCreateDto dto)
    {
        await _serviceService.AddAsync(dto);
        return Ok(new { message = "Service yaradıldı." });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, ServiceUpdateDto dto)
    {
        if (id != dto.Id)
            return BadRequest("ID uyğun deyil");

        var result = await _serviceService.UpdateAsync(dto);
        if (!result)
            return NotFound($"ID tapılmadı: {dto.Id}");

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _serviceService.DeleteAsync(id);
        return Ok(new { message = "Service silindi." });
    }
}
