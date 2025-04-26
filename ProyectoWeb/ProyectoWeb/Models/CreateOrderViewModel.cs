using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProyectoWeb.Models
{
    public class CreateOrderViewModel
    {
        [Required]
        [Display(Name = "Número de mesa")]
        public int TableNumber { get; set; }

        [Required]
        [Display(Name = "Forma de pago")]
        public string PaymentMethod { get; set; }

        public List<OrderItemViewModel> Items { get; set; } = new();
    }
}

