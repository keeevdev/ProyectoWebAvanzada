using System.Collections.Generic;

namespace ProyectoApi.Dtos
{
    /// <summary>
    /// DTO para crear pedido: mesa + lista de ítems.
    /// </summary>
    public record CreateOrderDto(int TableNumber, IEnumerable<OrderItemDto> Items);
}
