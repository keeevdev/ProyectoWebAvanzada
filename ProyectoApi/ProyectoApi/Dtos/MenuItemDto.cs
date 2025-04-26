namespace ProyectoApi.Dtos
{
    public record MenuItemDto(
        int Id,
        string Name,
        string? Description,
        decimal Price,
        string? ImageUrl,
        int? CategoryId,
        string? CategoryName
    );
}
