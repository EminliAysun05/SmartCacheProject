using CacheSmartProject.Domain.Dtos.Service;
using CacheSmartProject.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using SmartCacheProject.Application.Services.Interfaces;
using static System.Runtime.InteropServices.JavaScript.JSType;


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

    // GET: api/services?lastModified=2024-01-01T00:00:00Z
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] DateTime? lastModified)
    {
        var services = await _serviceService.GetServicesAlwaysAsync(lastModified);
        return Ok(services);
    }

    // POST: api/services
    [HttpPost]
    public async Task<IActionResult> Create(ServiceCreateDto dto)
    {
        await _serviceService.AddAsync(dto);
        return Ok(new { message = "Service yaradıldı." });
    }

    // PUT: api/services/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, ServiceUpdateDto dto)
    {
        if (id != dto.Id)
            return BadRequest("ID uyğun deyil.");

        await _serviceService.UpdateAsync(dto);
        return Ok(new { message = "Service yeniləndi." });
    }

    // DELETE: api/services/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _serviceService.DeleteAsync(id);
        return Ok(new { message = "Service silindi." });
    }
}
