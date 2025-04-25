using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using ProyectoApi.Helpers;
using ProyectoApi.Interfaces;
using ProyectoApi.Models;

namespace ProyectoApi.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DapperContext _context;

        public UserRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(User user, byte[] passwordHash)
        {
            using IDbConnection conn = _context.CreateConnection();
            await conn.ExecuteAsync(
                "sp_RegisterUser",
                new { user.Username, user.Email, PasswordHash = passwordHash },
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            using IDbConnection conn = _context.CreateConnection();
            return await conn.QuerySingleOrDefaultAsync<User>(
                "sp_GetUserByUsername",
                new { Username = username },
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<IEnumerable<string>> GetRolesAsync(int userId)
        {
            using IDbConnection conn = _context.CreateConnection();
            return await conn.QueryAsync<string>(
                "sp_GetRolesByUserId",
                new { UserId = userId },
                commandType: CommandType.StoredProcedure
            );
        }


        public async Task<User?> GetByIdAsync(int id)
        {
            using IDbConnection conn = _context.CreateConnection();
            const string sql = @"
                SELECT Id, Username, Email, PasswordHash, CreatedAt
                FROM Users
                WHERE Id = @Id";
            return await conn.QuerySingleOrDefaultAsync<User>(
                sql, new { Id = id }
            );
        }

        public async Task UpdateAsync(int id, string username, string email)
        {
            using IDbConnection conn = _context.CreateConnection();
            const string sql = @"
                UPDATE Users
                SET Username = @Username,
                    Email    = @Email
                WHERE Id = @Id";
            await conn.ExecuteAsync(
                sql, new { Id = id, Username = username, Email = email }
            );
        }
    }
}



