using System.ComponentModel.DataAnnotations.Schema;

namespace Feed_Bridge.Models.Entities
{
    public class Cart
    {
        public int Id { get; set; }

        public List<Order> Orders { get; set; }

        public List<ProductCart> ProductCarts { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
