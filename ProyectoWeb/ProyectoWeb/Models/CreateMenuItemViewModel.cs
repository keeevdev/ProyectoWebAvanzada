using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace ProyectoWeb.Models
{
    public class CreateMenuItemViewModel
    {
        [Required] public string Name { get; set; } = "";
        public string? Description { get; set; }

        [Required, Range(0.01, 10000)]
        public decimal Price { get; set; }

        [Display(Name = "Imagen")]
        public IFormFile? ImageFile { get; set; }   // nuevo para el upload

        // Se usara para mostrar y enviar al API
        public string? ImageUrl { get; set; }
    }
}
