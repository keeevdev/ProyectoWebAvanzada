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
    public class MenuController : ControllerBase
    {
        private readonly IMenuRepository _menuRepo;
        public MenuController(IMenuRepository menuRepo) => _menuRepo = menuRepo;

        // GET /api/menu?categoryId=…
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll(int? categoryId)
        {
            var items = await _menuRepo.GetAllAsync();
            if (categoryId.HasValue)
                items = items.Where(i => i.CategoryId == categoryId);

            var dtos = items.Select(i => new MenuItemDto(
                i.Id, i.Name, i.Description, i.Price, i.ImageUrl,
                i.CategoryId, i.CategoryName));
            return Ok(dtos);
        }

        // GET /api/menu/{id}
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var i = await _menuRepo.GetByIdAsync(id);
            if (i == null) return NotFound();
            var dto = new MenuItemDto(
                i.Id, i.Name, i.Description, i.Price, i.ImageUrl,
                i.CategoryId, i.CategoryName);
            return Ok(dto);
        }

        // POST /api/menu
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create([FromBody] CreateMenuItemDto dto)
        {
            var item = new MenuItem
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                ImageUrl = dto.ImageUrl,
                CategoryId = dto.CategoryId
            };
            await _menuRepo.AddAsync(item);
            return CreatedAtAction(nameof(GetById), new { id = item.Id }, null);
        }

        // PUT /api/menu/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateMenuItemDto dto)
        {
            var existing = await _menuRepo.GetByIdAsync(id);
            if (existing == null) return NotFound();
            existing.Name = dto.Name;
            existing.Description = dto.Description;
            existing.Price = dto.Price;
            existing.ImageUrl = dto.ImageUrl;
            existing.CategoryId = dto.CategoryId;
            await _menuRepo.UpdateAsync(existing);
            return NoContent();
        }

        // DELETE /api/menu/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int id)
        {
            await _menuRepo.DeleteAsync(id);
            return NoContent();
        }
    }
}




