using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using ProyectoApi.Helpers;
using ProyectoApi.Interfaces;
using ProyectoApi.Models;

namespace ProyectoApi.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly DapperContext _ctx;
        public OrderRepository(DapperContext ctx) => _ctx = ctx;

        public async Task<int> CreateOrderAsync(int userId, int tableNumber, string paymentMethod)
        {
            using var conn = _ctx.CreateConnection();
            var p = new DynamicParameters();
            p.Add("UserId", userId);
            p.Add("TableNumber", tableNumber);
            p.Add("PaymentMethod", paymentMethod);
            p.Add("OrderId", dbType: DbType.Int32, direction: ParameterDirection.Output);
            await conn.ExecuteAsync("sp_CreateOrder", p, commandType: CommandType.StoredProcedure);
            return p.Get<int>("OrderId");
        }

        public async Task AddOrderItemAsync(int orderId, int menuItemId, int quantity)
        {
            using var conn = _ctx.CreateConnection();
            await conn.ExecuteAsync(
                "sp_AddOrderItem",
                new { OrderId = orderId, MenuItemId = menuItemId, Quantity = quantity },
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<IEnumerable<Order>> GetPendingOrdersAsync()
        {
            using var conn = _ctx.CreateConnection();
            return await conn.QueryAsync<Order>(
                "sp_GetOrdersForEmployee", commandType: CommandType.StoredProcedure
            );
        }

        public async Task AssignOrderTimeAsync(int orderId, int minutes)
        {
            using var conn = _ctx.CreateConnection();
            await conn.ExecuteAsync(
                "sp_AssignOrderTime",
                new { OrderId = orderId, EstimatedTimeMinutes = minutes },
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task CompleteOrderAsync(int orderId)
        {
            using var conn = _ctx.CreateConnection();
            await conn.ExecuteAsync(
                "sp_CompleteOrder",
                new { OrderId = orderId },
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<IEnumerable<OrderItemDetail>> GetOrderItemsAsync(int orderId)
        {
            using var conn = _ctx.CreateConnection();
            return await conn.QueryAsync<OrderItemDetail>(
                "sp_GetOrderItems",
                new { OrderId = orderId },
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<IEnumerable<Order>> GetOrdersByUserAsync(int userId)
        {
            using var conn = _ctx.CreateConnection();
            return await conn.QueryAsync<Order>(
                "sp_GetOrdersByUser",
                new { UserId = userId },
                commandType: CommandType.StoredProcedure
            );
        }
    }
}

