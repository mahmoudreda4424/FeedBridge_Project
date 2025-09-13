using Feed_Bridge.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace Feed_Bridge.ViewModel
{
    public class ReportViewModel
    {
        [Required(ErrorMessage = "العنوان مطلوب")]
        [StringLength(150)]
        public string Title { get; set; }

        [Required(ErrorMessage = "المحتوى مطلوب")]
        public string Content { get; set; }

        //[Required(ErrorMessage = "البريد الإلكتروني مطلوب")]
        //[EmailAddress(ErrorMessage = "بريد إلكتروني غير صالح")]
        //public string Email { get; set; }

        [Required(ErrorMessage = "المتبرع مطلوب")]
        public string? SelectedUserId { get; set; } // هنخزن الـ UserId

        public List<ApplicationUser> Donors { get; set; } = new();

    }
}
