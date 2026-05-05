using System;
using System.ComponentModel.DataAnnotations;

namespace FoodDelivery.Models
{
    public enum PaymentStatus
    {
        Pending,
        SimulatedSuccess,
        ChapaSuccess,
        Failed
    }

    public class Payment
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

        [MaxLength(50)]
        public string Method { get; set; }

        [MaxLength(100)]
        public string TxRef { get; set; }

        public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;

        public virtual Order Order { get; set; }
    }
}
