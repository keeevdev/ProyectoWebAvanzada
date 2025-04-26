using ProyectoApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProyectoApi.Interfaces
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAllAsync();
        Task<Category?> GetByIdAsync(int id);
        Task AddAsync(Category cat);
        Task UpdateAsync(Category cat);
        Task DeleteAsync(int id);
    }
}

