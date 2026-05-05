using System.ComponentModel.DataAnnotations;
using FoodDelivery.Helpers;

namespace FoodDelivery.Models.ViewModels
{
    public class AdminUserViewModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; }
        public bool IsActive { get; set; }
    }

    public class DashboardViewModel
    {
        public int TotalUsers { get; set; }
        public int TotalOrders { get; set; }
        public int TotalMenuItems { get; set; }
    }

    public class AdminOrderViewModel
    {
        public int OrderId { get; set; }
        public string CustomerEmail { get; set; }
        public System.DateTime CreatedAt { get; set; }
        public decimal TotalAmount { get; set; }
        public FoodDelivery.Models.OrderStatus Status { get; set; }
    }

    public class CreateOwnerViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [RegularExpression(ValidationConstants.InternationalNameRegex, ErrorMessage = ValidationConstants.InternationalNameErrorMessage)]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
