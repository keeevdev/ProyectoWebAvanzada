using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoApi.Dtos;
using ProyectoApi.Interfaces;

namespace ProyectoApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderRepository _orderRepo;
        public OrdersController(IOrderRepository orderRepo) => _orderRepo = orderRepo;

        [HttpPost]
        [Authorize(Roles = "usuario")]
        public async Task<IActionResult> PlaceOrder(CreateOrderDto dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var orderId = await _orderRepo.CreateOrderAsync(userId, dto.TableNumber, dto.PaymentMethod);
            foreach (var it in dto.Items)
                await _orderRepo.AddOrderItemAsync(orderId, it.MenuItemId, it.Quantity);
            return CreatedAtAction(nameof(GetOrderItems), new { orderId });
        }

        [HttpGet("employee")]
        [Authorize(Roles = "empleado")]
        public async Task<IActionResult> GetPendingOrders() =>
            Ok(await _orderRepo.GetPendingOrdersAsync());

        [HttpGet("{orderId}/items")]
        [Authorize(Roles = "empleado")]
        public async Task<IActionResult> GetOrderItems(int orderId) =>
            Ok(await _orderRepo.GetOrderItemsAsync(orderId));

        [HttpPut("employee/assign/{orderId}")]
        [Authorize(Roles = "empleado")]
        public async Task<IActionResult> Assign(int orderId, AssignTimeDto dto)
        {
            await _orderRepo.AssignOrderTimeAsync(orderId, dto.EstimatedTimeMinutes);
            return NoContent();
        }

        [HttpPut("employee/complete/{orderId}")]
        [Authorize(Roles = "empleado")]
        public async Task<IActionResult> Complete(int orderId)
        {
            await _orderRepo.CompleteOrderAsync(orderId);
            return NoContent();
        }

        [HttpGet("user")]
        [Authorize(Roles = "usuario")]
        public async Task<IActionResult> GetMyOrders()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            return Ok(await _orderRepo.GetOrdersByUserAsync(userId));
        }
    }
}


