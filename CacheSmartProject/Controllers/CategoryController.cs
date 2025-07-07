using CacheSmartProject.Application.Services.Interfaces;
using CacheSmartProject.Domain.Dtos.Category;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CacheSmartProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] DateTime? lastModified)
        {
            var categories = await _categoryService.GetCategoriesAlwaysAsync(lastModified);
            return Ok(categories);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CategoryCreateDto dto)
        {
            await _categoryService.AddAsync(dto);
            return Ok(new { message = "Kateqoriya yaradıldı." });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CategoryUpdateDto dto)
        {
            if (id != dto.Id)
                return BadRequest("ID uyğun deyil.");

            await _categoryService.UpdateAsync(dto);
            return Ok(new { message = "Kateqoriya yeniləndi." });
        }

        // DELETE: api/category/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _categoryService.DeleteAsync(id);
            return Ok(new { message = "Kateqoriya silindi." });
        }
    }
}
