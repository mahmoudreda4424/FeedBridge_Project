using Feed_Bridge.CutomVaildation;
using System.ComponentModel.DataAnnotations;

namespace Feed_Bridge.ViewModel
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "الاسم مطلوب")]
        [Display(Name = "الاسم")]

        public string UserName { get; set; }

        [Display(Name = "البريد الالكتروني")]
        [Required(ErrorMessage = "البريد الإلكتروني مطلوب")]
        [EmailAddress]
        public string Email { get; set; }

        [Display(Name = "كلمه المرور")]
        [Required(ErrorMessage = "كلمة المرور مطلوبة")]
        [DataType(DataType.Password)]
        [Compare("ConfirmPassword", ErrorMessage = "كلمة السر غير متطابقة")]
        public string Password { get; set; }

        [Display(Name = "تأكيد كلمه المرور")]
        [Required(ErrorMessage = "تأكيد كلمة المرور مطلوب")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        [Display(Name = "رقم الهاتف")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "تاريخ الميلاد")]
        public DateOnly BirthDate { get; set; }

        [Display(Name = "الصورة الشخصية")]
        public IFormFile? ImgFile { get; set; }

        [Display(Name = "العنوان ")]
        public string? Address { get; set; }


    }
}
