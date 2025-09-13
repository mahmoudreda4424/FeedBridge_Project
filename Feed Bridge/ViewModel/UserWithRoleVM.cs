namespace Feed_Bridge.ViewModel
{
    public class UserWithRoleVM
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string ImgUrl { get; set; }
        public bool IsFrozen { get; set; }
        public bool IsDeleted { get; set; }
        public string? DeletedBy { get; set; }
        public IList<string> Roles { get; set; }
    }
}
