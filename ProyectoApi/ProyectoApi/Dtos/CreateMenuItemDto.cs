namespace ProyectoApi.Dtos
{
    public record CreateMenuItemDto(
        string Name,
        string? Description,
        decimal Price,
        string? ImageUrl,
        int? CategoryId
    );
}
