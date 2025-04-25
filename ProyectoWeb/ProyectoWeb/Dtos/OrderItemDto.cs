namespace ProyectoWeb.Dtos
{
    /// <summary>
    /// DTO que se envia al API para indicar qué ítem de menu y cuantas unidades
    /// </summary>
    public record OrderItemDto(int MenuItemId, int Quantity);
}
