using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using ProyectoApi.Helpers;
using ProyectoApi.Interfaces;
using ProyectoApi.Models;

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
            await conn.ExecuteAsync(
                "sp_AddMenuItem",
                new
                {
                    item.Name,
                    item.Description,
                    item.Price,
                    item.ImageUrl   // nuevo
                },
                commandType: CommandType.StoredProcedure);
        }

        public async Task UpdateAsync(MenuItem item)
        {
            using var conn = _ctx.CreateConnection();
            await conn.ExecuteAsync(
                "sp_UpdateMenuItem",
                new
                {
                    item.Id,
                    item.Name,
                    item.Description,
                    item.Price,
                    item.ImageUrl   // nuevo
                },
                commandType: CommandType.StoredProcedure);
        }

        public async Task DeleteAsync(int id)
        {
            using var conn = _ctx.CreateConnection();
            await conn.ExecuteAsync(
                "sp_DeleteMenuItem",
                new { Id = id },
                commandType: CommandType.StoredProcedure);
        }
    }
}

