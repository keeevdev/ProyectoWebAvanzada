namespace ProyectoApi.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int TableNumber { get; set; }
        public required string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? EstimatedTimeMinutes { get; set; }
        public string PaymentMethod { get; set; }
    }
}
