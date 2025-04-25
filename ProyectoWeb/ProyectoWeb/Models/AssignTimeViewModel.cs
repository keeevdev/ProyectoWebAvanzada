using System.ComponentModel.DataAnnotations;

namespace ProyectoWeb.Models
{
    public class AssignTimeViewModel
    {
        [Required]
        [Display(Name = "Tiempo estimado (min)")]
        public int EstimatedTimeMinutes { get; set; }
    }
}
