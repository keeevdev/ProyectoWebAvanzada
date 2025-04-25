using ProyectoApi.Models;
namespace ProyectoApi.Interfaces
{
    public interface IUserRepository
    {
        Task CreateAsync(User user, byte[] passwordHash);
        Task<User?> GetByUsernameAsync(string username);
        Task<IEnumerable<string>> GetRolesAsync(int userId);
        Task<User?> GetByIdAsync(int id);
        Task UpdateAsync(int id, string username, string email);
    }
}


