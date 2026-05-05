using System;
using System.Collections.Generic;

namespace FoodDelivery.Models.ViewModels
{
    public class DashboardOrderViewModel
    {
        public int OrderId { get; set; }
        public string CustomerName { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; }
        public OrderStatus? NextStatus { get; set; }
    }

    public class RestaurantDashboardViewModel
    {
        public Restaurant Restaurant { get; set; }
        public List<DashboardOrderViewModel> Orders { get; set; } = new List<DashboardOrderViewModel>();
        public string StatusFilter { get; set; }
    }

    public class OrderStatusUpdateViewModel
    {
        public int OrderId { get; set; }
        public OrderStatus NewStatus { get; set; }
    }
}
