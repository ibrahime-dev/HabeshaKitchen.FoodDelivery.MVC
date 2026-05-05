using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodDelivery.Models
{
    public enum OrderStatus
    {
        Pending,
        Confirmed,
        Preparing,
        Delivered,
        Cancelled,
        Rejected
    }

    public class Order
    {
        public int Id { get; set; }

        [Required]
        public string CustomerId { get; set; }

        public int RestaurantId { get; set; }

        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public decimal TotalAmount { get; set; }

        [Required, MaxLength(200)]
        public string Street { get; set; }

        [Required, MaxLength(100)]
        public string City { get; set; }

        [Required, MaxLength(20)]
        public string PostalCode { get; set; }

        [ForeignKey("CustomerId")]
        public virtual ApplicationUser Customer { get; set; }

        public virtual Restaurant Restaurant { get; set; }

        public virtual ICollection<OrderItem> OrderItems { get; set; }

        public virtual Payment Payment { get; set; }
    }
}
