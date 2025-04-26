using ProyectoApi.Dtos;
using ProyectoApi.Models;
using System.Threading.Tasks;
namespace ProyectoApi.Interfaces
{
    public interface IUserRepository
    {
        Task CreateAsync(User user, byte[] passwordHash);
        Task<User?> GetByUsernameAsync(string username);
        Task<IEnumerable<string>> GetRolesAsync(int userId);
        Task<User?> GetByIdAsync(int id);
        Task UpdateAsync(int id, string username, string email);
        Task<IEnumerable<User>> GetAllAsync();
        Task UpdateFullAsync(int id, string username, string email, byte[]? passwordHash, string role);
        Task UpdatePasswordAsync(int id, byte[] passwordHash);
        Task UpdateRoleAsync(int userId, int roleId);
        Task<UpdateUserDto?> GetByIdWithRoleAsync(int id);
    }
}


