using ProyectoApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProyectoApi.Interfaces
{
    public interface IMenuRepository
    {
        Task<IEnumerable<MenuItem>> GetAllAsync();
        Task<MenuItem?> GetByIdAsync(int id);
        Task AddAsync(MenuItem item);
        Task UpdateAsync(MenuItem item);
        Task DeleteAsync(int id);
    }
}
