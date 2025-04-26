using System.ComponentModel.DataAnnotations;

namespace ProyectoWeb.Models
{
    public class UserViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Nombre de usuario")]
        public string Username { get; set; } = "";

        [Required]
        [EmailAddress]
        [Display(Name = "Correo electrónico")]
        public string Email { get; set; } = "";

        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string? Password { get; set; }

        [Display(Name = "Rol")]
        public int RoleId { get; set; }
    }

}
