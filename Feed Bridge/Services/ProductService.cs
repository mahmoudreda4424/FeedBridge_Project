using Feed_Bridge.IServices;
using Feed_Bridge.Models.Data;
using Feed_Bridge.Models.Entities;
using Feed_Bridge.Models.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Feed_Bridge.Services
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _context;

        public ProductService(AppDbContext context)
        {
            _context = context;
        }

        // النسخة اللي بتاخد كاتيجوري
        public async Task<IEnumerable<Product>> GetAllAsync(string category)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            var query = _context.Products.AsQueryable();

            // فلترة حسب تاريخ الانتهاء
            query = query.Where(x => x.ExpirDate > today);

            // فلترة حسب الكاتيجوري (case-insensitive)
            if (!string.IsNullOrWhiteSpace(category) &&
                Enum.TryParse<ProductCategory>(category, true, out var parsedCategory))
            {
                query = query.Where(x => x.Category == parsedCategory);
            }

            return await query
                         .AsNoTracking()
                         .ToListAsync();
        }

        // النسخة اللي من غير كاتيجوري
        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await GetAllAsync(null);
        }


        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.Products
                                 .Include(p => p.Donation)
                                 .AsNoTracking()
                                 .FirstOrDefaultAsync(p => p.Id == id);
        }

        // إرجاع المنتج المرتبط بتبرع معين (مفيد لمنع التكرار)
        public async Task<Product?> GetByDonationId(int donationId)
        {
            return await _context.Products
                                 .Include(p => p.Donation)
                                 .AsNoTracking()
                                 .FirstOrDefaultAsync(p => p.DonationId == donationId);
        }

        public async Task AddAsync(Product product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }
    }
}
