using System.Collections.Generic;

namespace ProyectoWeb.Dtos
{
    /// <summary>
    /// DTO para crear un pedido: numero de mesa y lista de itemss
    /// </summary>
    public record CreateOrderDto(int TableNumber, string PaymentMethod, IEnumerable<OrderItemDto> Items);
}
