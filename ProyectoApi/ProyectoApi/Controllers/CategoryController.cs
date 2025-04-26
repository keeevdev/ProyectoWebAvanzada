using Microsoft.AspNetCore.Mvc;
using ProyectoApi.Dtos;
using ProyectoApi.Interfaces;
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

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var cats = await _repo.GetAllAsync();
            return Ok(cats.Select(c => new CategoryDto(c.Id, c.Name)));
        }
    }
}
