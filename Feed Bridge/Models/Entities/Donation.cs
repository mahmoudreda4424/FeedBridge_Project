using Feed_Bridge.Models.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Feed_Bridge.Models.Entities
{
    public class Donation
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string? ImgURL { get; set; }
        public DateOnly ExpirDate { get; set; }
        public decimal Quantity { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;
        public ProductCategory Category { get; set; }

        // 🟢 حالة التبرع (مطلوبة عشان قبول / رفض التبرع)
        public DonationStatus Status { get; set; } = DonationStatus.Pending;

        [ForeignKey("User")]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        [ForeignKey("Delivery")]
        public string? DeliveryId { get; set; }
        public ApplicationUser? Delivery { get; set; }

        public List<Product> Product { get; set; } = new List<Product>();
    }
}
