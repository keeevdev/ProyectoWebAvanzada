using ProyectoApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProyectoApi.Interfaces
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAllAsync();
    }
}
