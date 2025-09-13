using System.ComponentModel.DataAnnotations.Schema;

namespace Feed_Bridge.Models.Entities
{
    public class Partener
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public string? RedirectUrl { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
