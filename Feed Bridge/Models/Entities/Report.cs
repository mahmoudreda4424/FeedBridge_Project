using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Feed_Bridge.Models.Entities
{
    public class Report
    {
        public int Id { get; set; }
        public string title { get; set; }
        public string content { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Email { get; set; }
        [ForeignKey("User")]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
