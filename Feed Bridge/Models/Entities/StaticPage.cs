using System.ComponentModel.DataAnnotations.Schema;

namespace Feed_Bridge.Models.Entities
{
    public class StaticPage
    {
        public int Id { get; set; }
        public string Content1 { get; set; } = "";
        public string Content2 { get; set; } = "";
        public string? VideoUrl { get; set; }
        public string? PartenerBackgroundImageUrl { get; set; } = "";
        public string? HomePageBackgroundImageUrl { get; set; } = "";
        [ForeignKey("User")]
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }
    }
}
