using Feed_Bridge.Models.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Feed_Bridge.Models.Entities
{
    public class Support
    {
        public int Id { get; set; }

        public decimal Amount { get; set; }

        public DateTime PaymentDate { get; set; }

        // Card
        public string PaymentMethod { get; set; }

        // البوابه اللي بتعامل معاه هي اللي هتبعته ودا بيكون رقم العمليه 
        public string TransactionId { get; set; }

        // Success, Pending, Failed, Refunded
        public PaymentStatus Status { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
