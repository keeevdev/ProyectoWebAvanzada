using ProyectoApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProyectoApi.Interfaces
{
    public interface IOrderRepository
    {
        Task<int> CreateOrderAsync(int userId, int tableNumber, string paymentMethod);
        Task AddOrderItemAsync(int orderId, int menuItemId, int quantity);
        Task<IEnumerable<Order>> GetPendingOrdersAsync();
        Task AssignOrderTimeAsync(int orderId, int minutes);
        Task CompleteOrderAsync(int orderId);
        Task<IEnumerable<OrderItemDetail>> GetOrderItemsAsync(int orderId);
        Task<IEnumerable<Order>> GetOrdersByUserAsync(int userId);

    }
}
