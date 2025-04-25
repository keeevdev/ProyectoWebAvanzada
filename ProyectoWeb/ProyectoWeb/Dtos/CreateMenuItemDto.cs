namespace ProyectoWeb.Dtos
{
    /// <summary>
    /// DTO para crear o actualizar un ítem de menu en la llamada al API.
    /// </summary>
    public record CreateMenuItemDto(
        string Name,
        string? Description,
        decimal Price,
        string? ImageUrl
    );
}
