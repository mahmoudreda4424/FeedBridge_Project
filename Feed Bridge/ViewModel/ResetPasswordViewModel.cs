using System.ComponentModel.DataAnnotations;

namespace Feed_Bridge.ViewModel
{
    public class ResetPasswordViewModel
    {
        public string Email { get; set; }
        public string Token { get; set; }

        [Compare("ConfirmPassword", ErrorMessage = "كلمه المرور غير متطابقه")]
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
