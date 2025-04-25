using System.ComponentModel.DataAnnotations;

namespace ProyectoWeb.Models
{
    public class ProfileViewModel
    {
        [Required]
        [Display(Name = "Nombre de usuario")]
        public string Username { get; set; } = "";

        [Required]
        [EmailAddress]
        [Display(Name = "Correo electrónico")]
        public string Email { get; set; } = "";
    }
}
