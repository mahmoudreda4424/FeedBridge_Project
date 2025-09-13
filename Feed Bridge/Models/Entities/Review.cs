using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Feed_Bridge.Models.Entities
{
    public class Review
    {
        public int Id { get; set; }
        public string  FeedBackMsg { get; set; }
        public DateTime  CreatedAt{ get; set; }
        public int StarsNumber { get; set; }
        [ForeignKey("User")]
        public string UserID { get; set; }
        public ApplicationUser User { get; set; }
    }
}
