namespace ProyectoApi.Dtos
{
    /// <summary>
    /// Item de pedido (ID de menu + cantidad).
    /// </summary>
    public record OrderItemDto(int MenuItemId, int Quantity);
}
