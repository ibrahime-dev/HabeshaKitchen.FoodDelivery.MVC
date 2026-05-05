using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FoodDelivery.Helpers;

namespace FoodDelivery.Models.ViewModels
{
    public class RestaurantIndexViewModel
    {
        public List<Restaurant> Restaurants { get; set; } = new List<Restaurant>();
        public string SearchTerm { get; set; }
        public string CategoryFilter { get; set; }
        public List<string> Categories { get; set; } = new List<string>();
    }

    public class RestaurantDetailsViewModel
    {
        public Restaurant Restaurant { get; set; }
        public System.Collections.Generic.Dictionary<string, List<MenuItem>> MenuItemsByCategory { get; set; }
        public string SearchTerm { get; set; }
    }

    public class RestaurantCreateEditViewModel
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        [RegularExpression(ValidationConstants.InternationalAddressRegex, ErrorMessage = ValidationConstants.InternationalAddressErrorMessage)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        [Required, MaxLength(50)]
        [RegularExpression(ValidationConstants.InternationalNameRegex, ErrorMessage = ValidationConstants.InternationalNameErrorMessage)]
        [Display(Name = "Cuisine Category")]
        public string CuisineCategory { get; set; }

        [Required, MaxLength(20)]
        [RegularExpression(ValidationConstants.EthiopianPhoneRegex, ErrorMessage = ValidationConstants.EthiopianPhoneErrorMessage)]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
    }
}
