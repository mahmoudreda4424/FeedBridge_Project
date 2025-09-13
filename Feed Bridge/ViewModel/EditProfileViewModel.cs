namespace Feed_Bridge.ViewModel
{
    public class EditProfileViewModel
    {
        public string? CurrentImgUrl { get; set; }

        public IFormFile? ImgFile { get; set; }
        public DateTime? BirthDate { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
    }
}
