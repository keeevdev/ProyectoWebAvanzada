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
    }
}
