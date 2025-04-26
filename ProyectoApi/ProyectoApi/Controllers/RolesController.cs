using Microsoft.AspNetCore.Mvc;
using ProyectoApi.Helpers;
using System.Data;
using Dapper;

namespace ProyectoApi.Controllers
{
    [ApiController]
    [Route("api/roles")]
    public class RolesController : ControllerBase
    {
        private readonly DapperContext _context;

        public RolesController(DapperContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            using IDbConnection conn = _context.CreateConnection();
            var roles = await conn.QueryAsync<RoleDto>("SELECT Id, Name FROM Roles");
            return Ok(roles);
        }
    }

    public class RoleDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
    }
}
