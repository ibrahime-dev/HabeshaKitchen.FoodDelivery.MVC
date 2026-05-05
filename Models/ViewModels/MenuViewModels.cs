using System.ComponentModel.DataAnnotations;
using System.Web;
using FoodDelivery.Helpers;

namespace FoodDelivery.Models.ViewModels
{
    public class MenuItemViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; }
        public bool IsAvailable { get; set; }
        public int RestaurantId { get; set; }
        public string ImageUrl { get; set; }
    }

    public class MenuItemCreateEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Item name is required.")]
        [MaxLength(100)]
        [RegularExpression(@"^[a-zA-Z\s\-\.']+$", ErrorMessage = "Item name must contain only letters, spaces, hyphens, periods, and apostrophes.")]
        [Display(Name = "Item Name")]
        public string Name { get; set; }

        [MaxLength(500)]
        [RegularExpression(@"^[a-zA-Z\s\-\.']+$", ErrorMessage = "Description must contain only letters, spaces, hyphens, periods, and apostrophes.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        [Display(Name = "Price (ETB)")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Category is required.")]
        [MaxLength(50)]
        [RegularExpression(@"^[a-zA-Z\s\-\.'&]+$", ErrorMessage = "Category must contain only letters, spaces, and basic punctuation.")]
        public string Category { get; set; }

        public bool IsAvailable { get; set; } = true;

        [Display(Name = "Item Image")]
        public HttpPostedFileBase ImageFile { get; set; }

        public string ImageUrl { get; set; }
    }
}
