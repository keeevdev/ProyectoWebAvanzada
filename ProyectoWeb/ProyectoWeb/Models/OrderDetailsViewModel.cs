using System.Collections.Generic;

namespace ProyectoWeb.Models
{
    public class OrderDetailsViewModel
    {
        public int OrderId { get; set; }
        public List<OrderItemDetailViewModel> Items { get; set; } = new();
        public AssignTimeViewModel AssignTime { get; set; } = new();
    }
}
