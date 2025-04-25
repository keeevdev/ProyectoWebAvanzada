namespace ProyectoApi.Models
{
    public class OrderItemDetail
    {
        public int OrderId { get; set; }
        public int MenuItemId { get; set; }
        public required string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}

