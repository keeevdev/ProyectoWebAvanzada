using Dapper;
using ProyectoApi.Helpers;
using ProyectoApi.Interfaces;
using ProyectoApi.Models;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace ProyectoApi.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly DapperContext _ctx;
        public CategoryRepository(DapperContext ctx) => _ctx = ctx;

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            using var conn = _ctx.CreateConnection();
            return await conn.QueryAsync<Category>("sp_GetCategories", commandType: CommandType.StoredProcedure);
        }

        public async Task<Category?> GetByIdAsync(int id)
        {
            using var conn = _ctx.CreateConnection();
            // Podemos usar la misma SP de listado, filtrando en memoria o crear sp_GetCategoryById. 
            // Aquí filtramos en memoria:
            var all = await GetAllAsync();
            return all.FirstOrDefault(c => c.Id == id);
        }

        public async Task AddAsync(Category cat)
        {
            using var conn = _ctx.CreateConnection();
            await conn.ExecuteAsync(
                "sp_AddCategory",
                new { Name = cat.Name },
                commandType: CommandType.StoredProcedure);
        }

        public async Task UpdateAsync(Category cat)
        {
            using var conn = _ctx.CreateConnection();
            await conn.ExecuteAsync(
                "sp_UpdateCategory",
                new { Id = cat.Id, Name = cat.Name },
                commandType: CommandType.StoredProcedure);
        }

        public async Task DeleteAsync(int id)
        {
            using var conn = _ctx.CreateConnection();
            await conn.ExecuteAsync(
                "sp_DeleteCategory",
                new { Id = id },
                commandType: CommandType.StoredProcedure);
        }
    }
}

