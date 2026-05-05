using System.Collections.Generic;
using System.Linq;

namespace FoodDelivery.Models.ViewModels
{
    public class CartItemViewModel
    {
        public int CartItemId { get; set; }
        public int MenuItemId { get; set; }
        public string MenuItemName { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal Subtotal => UnitPrice * Quantity;
        public int RestaurantId { get; set; }
        public string RestaurantName { get; set; }
    }

    public class CartViewModel
    {
        public int CartId { get; set; }
        public List<CartItemViewModel> Items { get; set; } = new List<CartItemViewModel>();
        public decimal Total => Items.Sum(i => i.Subtotal);
        public bool IsEmpty => Items.Count == 0;
        public string RestaurantName { get; set; }
        public int? RestaurantId { get; set; }
    }
}
