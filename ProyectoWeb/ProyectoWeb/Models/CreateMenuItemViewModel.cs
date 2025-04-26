using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace ProyectoWeb.Models
{
    public class CreateMenuItemViewModel
    {
        [Required] public string Name { get; set; } = "";
        public string? Description { get; set; }
        [Required][Range(0.01, 9999)] public decimal Price { get; set; }
        public IFormFile? ImageFile { get; set; }
        public string? ImageUrl { get; set; }
        public int? CategoryId { get; set; }
    }
}

