using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using ProyectoApi.Dtos;
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
        public async Task<IEnumerable<User>> GetAllAsync()
        {
            using IDbConnection conn = _context.CreateConnection();
            const string sql = @"SELECT Id, Username, Email, PasswordHash, CreatedAt FROM Users";
            return await conn.QueryAsync<User>(sql);
        }
        public async Task UpdateFullAsync(int id, string username, string email, byte[]? newPasswordHash, string newRole)
        {
            using IDbConnection conn = _context.CreateConnection();
            using var transaction = conn.BeginTransaction();

            const string updateUserSql = @"
        UPDATE Users
        SET Username = @Username,
            Email = @Email
        WHERE Id = @Id";

            await conn.ExecuteAsync(updateUserSql, new { Id = id, Username = username, Email = email }, transaction);

            if (newPasswordHash != null)
            {
                const string updatePasswordSql = @"
            UPDATE Users
            SET PasswordHash = @PasswordHash
            WHERE Id = @Id";

                await conn.ExecuteAsync(updatePasswordSql, new { Id = id, PasswordHash = newPasswordHash }, transaction);
            }

            const string roleIdSql = "SELECT Id FROM Roles WHERE Name = @Role";
            var roleId = await conn.ExecuteScalarAsync<int>(roleIdSql, new { Role = newRole }, transaction);

            const string deleteRolesSql = "DELETE FROM UserRoles WHERE UserId = @Id";
            await conn.ExecuteAsync(deleteRolesSql, new { Id = id }, transaction);

            const string insertRoleSql = "INSERT INTO UserRoles (UserId, RoleId) VALUES (@UserId, @RoleId)";
            await conn.ExecuteAsync(insertRoleSql, new { UserId = id, RoleId = roleId }, transaction);

            transaction.Commit();
        }
        public async Task UpdatePasswordAsync(int id, byte[] passwordHash)
        {
            using IDbConnection conn = _context.CreateConnection();
            const string sql = @"
        UPDATE Users
        SET PasswordHash = @PasswordHash
        WHERE Id = @Id";
            await conn.ExecuteAsync(sql, new { Id = id, PasswordHash = passwordHash });
        }

        public async Task UpdateRoleAsync(int userId, int roleId)
        {
            using IDbConnection conn = _context.CreateConnection();
            const string sqlDelete = "DELETE FROM UserRoles WHERE UserId = @UserId";
            const string sqlInsert = "INSERT INTO UserRoles (UserId, RoleId) VALUES (@UserId, @RoleId)";

            await conn.ExecuteAsync(sqlDelete, new { UserId = userId });
            await conn.ExecuteAsync(sqlInsert, new { UserId = userId, RoleId = roleId });
        }

        public async Task<UpdateUserDto?> GetByIdWithRoleAsync(int id)
        {
            using IDbConnection conn = _context.CreateConnection();

            const string sql = @"
        SELECT u.Id, u.Username, u.Email, ur.RoleId
        FROM Users u
        LEFT JOIN UserRoles ur ON u.Id = ur.UserId
        WHERE u.Id = @Id";

            return await conn.QueryFirstOrDefaultAsync<UpdateUserDto>(sql, new { Id = id });
        }
    }
}



