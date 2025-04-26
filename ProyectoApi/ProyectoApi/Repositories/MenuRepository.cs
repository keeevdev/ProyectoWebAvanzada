using Dapper;
using ProyectoApi.Helpers;
using ProyectoApi.Interfaces;
using ProyectoApi.Models;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace ProyectoApi.Repositories
{
    public class MenuRepository : IMenuRepository
    {
        private readonly DapperContext _ctx;
        public MenuRepository(DapperContext ctx) => _ctx = ctx;

        public async Task<IEnumerable<MenuItem>> GetAllAsync()
        {
            using var conn = _ctx.CreateConnection();
            return await conn.QueryAsync<MenuItem>(
                "sp_GetMenuItems", commandType: CommandType.StoredProcedure);
        }

        public async Task<MenuItem?> GetByIdAsync(int id)
        {
            using var conn = _ctx.CreateConnection();
            return await conn.QuerySingleOrDefaultAsync<MenuItem>(
                "sp_GetMenuItemById",
                new { Id = id },
                commandType: CommandType.StoredProcedure);
        }

        public async Task AddAsync(MenuItem item)
        {
            using var conn = _ctx.CreateConnection();
            var p = new DynamicParameters();
            p.Add("Name", item.Name);
            p.Add("Description", item.Description);
            p.Add("Price", item.Price);
            p.Add("ImageUrl", item.ImageUrl);
            p.Add("CategoryId", item.CategoryId);
            await conn.ExecuteAsync("sp_AddMenuItem", p, commandType: CommandType.StoredProcedure);
        }

        public async Task UpdateAsync(MenuItem item)
        {
            using var conn = _ctx.CreateConnection();
            var p = new DynamicParameters();
            p.Add("Id", item.Id);
            p.Add("Name", item.Name);
            p.Add("Description", item.Description);
            p.Add("Price", item.Price);
            p.Add("ImageUrl", item.ImageUrl);
            p.Add("CategoryId", item.CategoryId);
            await conn.ExecuteAsync("sp_UpdateMenuItem", p, commandType: CommandType.StoredProcedure);
        }

        public async Task DeleteAsync(int id)
        {
            using var conn = _ctx.CreateConnection();
            await conn.ExecuteAsync("DELETE FROM MenuItems WHERE Id = @Id", new { Id = id });
        }
    }
}


