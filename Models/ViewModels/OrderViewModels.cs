using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FoodDelivery.Helpers;

namespace FoodDelivery.Models.ViewModels
{
    public class CheckoutViewModel
    {
        [Required(ErrorMessage = "Street address is required.")]
        [MaxLength(200)]
        [RegularExpression(ValidationConstants.InternationalAddressRegex, ErrorMessage = ValidationConstants.InternationalAddressErrorMessage)]
        [Display(Name = "Street Address")]
        public string Street { get; set; }

        [Required(ErrorMessage = "City is required.")]
        [MaxLength(100)]
        [RegularExpression(ValidationConstants.InternationalNameRegex, ErrorMessage = ValidationConstants.InternationalNameErrorMessage)]
        public string City { get; set; }

        [Required(ErrorMessage = "Postal code is required.")]
        [MaxLength(20)]
        [RegularExpression(ValidationConstants.InternationalPostalCodeRegex, ErrorMessage = ValidationConstants.InternationalPostalCodeErrorMessage)]
        [Display(Name = "Postal Code")]
        public string PostalCode { get; set; }

        [Display(Name = "Payment Method")]
        public string PaymentMethod { get; set; } = "Simulated";

        [Display(Name = "Transaction Reference")]
        [MaxLength(100)]
        public string TransactionRef { get; set; }

        public List<CartItemViewModel> CartItems { get; set; } = new List<CartItemViewModel>();
        public decimal Total { get; set; }
    }

    public class OrderItemViewModel
    {
        public string MenuItemName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal => UnitPrice * Quantity;
    }

    public class OrderConfirmationViewModel
    {
        public int OrderId { get; set; }
        public string RestaurantName { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal TotalAmount { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public OrderStatus Status { get; set; }
        public string PaymentStatus { get; set; }
        public List<OrderItemViewModel> Items { get; set; } = new List<OrderItemViewModel>();
    }

    public class OrderHistoryViewModel
    {
        public int OrderId { get; set; }
        public string RestaurantName { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; }
        public List<OrderItemViewModel> Items { get; set; } = new List<OrderItemViewModel>();
    }
}
