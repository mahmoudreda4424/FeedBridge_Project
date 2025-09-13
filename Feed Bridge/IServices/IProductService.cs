using Feed_Bridge.Models.Entities;

namespace Feed_Bridge.IServices
{
    public interface IProductService
    {
        Task AddAsync(Product product);
        Task<IEnumerable<Product>> GetAllAsync(); // النسخة العادية
        Task<IEnumerable<Product>> GetAllAsync(string category); // 🟢 النسخة اللي تاخد كاتيجوري
        Task<Product> GetByIdAsync(int id);
        Task UpdateAsync(Product product);
        Task DeleteAsync(int id);

        // 🆕 الميثود الجديدة عشان تجيب المنتج المرتبط بتبرع معين
        Task<Product?> GetByDonationId(int donationId);
    }
}
