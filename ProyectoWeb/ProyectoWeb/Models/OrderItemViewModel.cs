namespace ProyectoWeb.Models
{
    public class OrderItemViewModel
    {
        public int MenuItemId { get; set; }
        public string Name { get; set; } = "";
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
