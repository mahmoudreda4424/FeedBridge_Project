namespace Feed_Bridge.ViewModel
{
    public class UserWithRolesViewModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public IList<string> Roles { get; set; } = new List<string>();
        public int OrdersCount { get; set; }
        public int SupportsCount { get; set; }
    }
}
