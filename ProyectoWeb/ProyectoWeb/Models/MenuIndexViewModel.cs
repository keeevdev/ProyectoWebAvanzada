using System.Collections.Generic;
namespace ProyectoWeb.Models
{
    public class MenuIndexViewModel
    {
        public List<CategoryViewModel> Categories { get; set; } = new();
        public int? SelectedCategoryId { get; set; }
        public List<MenuItemViewModel> Items { get; set; } = new();
    }
}
