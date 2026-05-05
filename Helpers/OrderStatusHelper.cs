using FoodDelivery.Models;

namespace FoodDelivery.Helpers
{
    public static class OrderStatusHelper
    {
        public static OrderStatus? GetNextStatus(OrderStatus current)
        {
            switch (current)
            {
                case OrderStatus.Pending:   return OrderStatus.Confirmed;
                case OrderStatus.Confirmed: return OrderStatus.Preparing;
                case OrderStatus.Preparing: return OrderStatus.Delivered;
                default: return null;
            }
        }

        public static bool IsValidTransition(OrderStatus current, OrderStatus next)
        {
            if (next == OrderStatus.Cancelled)
                return current == OrderStatus.Pending || current == OrderStatus.Confirmed;

            if (next == OrderStatus.Rejected)
                return current == OrderStatus.Pending;

            return GetNextStatus(current) == next;
        }

        public static string GetBadgeClass(OrderStatus status)
        {
            switch (status)
            {
                case OrderStatus.Pending:   return "bg-warning text-dark";
                case OrderStatus.Confirmed: return "bg-info text-dark";
                case OrderStatus.Preparing: return "bg-primary";
                case OrderStatus.Delivered: return "bg-success";
                case OrderStatus.Cancelled: return "bg-secondary";
                case OrderStatus.Rejected:  return "bg-danger";
                default: return "bg-secondary";
            }
        }
    }

    public static class ValidationConstants
    {
        public const string InternationalNameRegex = @"^[\p{L}\p{M}\s\-\.']+$";
        public const string InternationalNameErrorMessage = "Only letters, spaces, hyphens, periods, and apostrophes are allowed.";

        public const string InternationalAddressRegex = @"^[\p{L}\p{N}\s\-\.,'#]+$";
        public const string InternationalAddressErrorMessage = "Contains invalid characters. Only letters, numbers, spaces, and basic punctuation are allowed.";

        public const string EthiopianPhoneRegex = @"^(?:\+251|0)[1-9]\d{8}$";
        public const string EthiopianPhoneErrorMessage = "Must be a valid Ethiopian phone number (e.g. 0911234567 or +251911234567).";

        public const string InternationalPostalCodeRegex = @"^[A-Za-z0-9\s\-]{3,10}$";
        public const string InternationalPostalCodeErrorMessage = "Must be a valid postal code.";
    }
}
