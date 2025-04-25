using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoApi.Dtos;
using ProyectoApi.Interfaces;

namespace ProyectoApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MenuController : ControllerBase
    {
        private readonly IMenuRepository _menuRepo;
        public MenuController(IMenuRepository menuRepo) => _menuRepo = menuRepo;

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var items = await _menuRepo.GetAllAsync();
            var dtos = items.Select(i =>
                new MenuItemDto(i.Id, i.Name, i.Description, i.Price, i.ImageUrl));
            return Ok(dtos);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var i = await _menuRepo.GetByIdAsync(id);
            if (i is null) return NotFound();
            return Ok(new MenuItemDto(i.Id, i.Name, i.Description, i.Price, i.ImageUrl));
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Add([FromBody] CreateMenuItemDto dto)
        {
            var item = new Models.MenuItem
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                ImageUrl = dto.ImageUrl
            };
            await _menuRepo.AddAsync(item);
            return CreatedAtAction(nameof(GetById), new { id = item.Id }, null);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateMenuItemDto dto)
        {
            var existing = await _menuRepo.GetByIdAsync(id);
            if (existing is null) return NotFound();

            existing.Name = dto.Name;
            existing.Description = dto.Description;
            existing.Price = dto.Price;
            existing.ImageUrl = dto.ImageUrl;

            await _menuRepo.UpdateAsync(existing);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int id)
        {
            await _menuRepo.DeleteAsync(id);
            return NoContent();
        }
    }
}


