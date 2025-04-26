using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoApi.Dtos;
using ProyectoApi.Interfaces;
using ProyectoApi.Models;
using System.Linq;
using System.Threading.Tasks;

namespace ProyectoApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository _repo;
        public CategoryController(ICategoryRepository repo) => _repo = repo;

        // GET: api/category
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var cats = await _repo.GetAllAsync();
            return Ok(cats.Select(c => new CategoryDto(c.Id, c.Name)));
        }

        // GET: api/category/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var cat = await _repo.GetByIdAsync(id);
            if (cat == null) return NotFound();
            return Ok(new CategoryDto(cat.Id, cat.Name));
        }

        // POST: api/category
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create([FromBody] CreateCategoryDto dto)
        {
            var cat = new Category { Name = dto.Name };
            await _repo.AddAsync(cat);
            return CreatedAtAction(nameof(GetById), new { id = cat.Id }, null);
        }

        // PUT: api/category/5
        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateCategoryDto dto)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return NotFound();
            existing.Name = dto.Name;
            await _repo.UpdateAsync(existing);
            return NoContent();
        }

        // DELETE: api/category/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int id)
        {
            await _repo.DeleteAsync(id);
            return NoContent();
        }
    }
}

