using Feed_Bridge.Models.Enums;

namespace Feed_Bridge.ViewModel
{
    public class EditProductViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateOnly ExpirDate { get; set; }
        public decimal Quantity { get; set; }

        // الحقول اللي محتاجاها
        public string Address { get; set; }
        public string Phone { get; set; }
        public string? Description { get; set; }
        public ProductCategory Category { get; set; }


        // الصورة
        public IFormFile? Image { get; set; }
        public string? ExistingImageUrl { get; set; }
    }
}
