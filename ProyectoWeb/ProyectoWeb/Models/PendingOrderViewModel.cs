using System;

namespace ProyectoWeb.Models
{
    public class PendingOrderViewModel
    {
        public int Id { get; set; }
        public int TableNumber { get; set; }
        public string PaymentMethod { get; set; } = "";
        public string Status { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public int? EstimatedTimeMinutes { get; set; }
    }
}
