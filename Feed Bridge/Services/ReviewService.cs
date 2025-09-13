using Feed_Bridge.IServices;
using Feed_Bridge.Models.Data;
using Feed_Bridge.Models.Entities;
using Microsoft.EntityFrameworkCore;  
using System.Threading.Tasks;          
using System.Collections.Generic;     

namespace Feed_Bridge.Services
{
    public class ReviewService : IReviewService
    {
        private readonly AppDbContext _context;

        public ReviewService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Review>> GetAllAsync()
        {
            return await _context.Reviews.Include(r => r.User).ToListAsync();
        }
        public async Task AddAsync(Review review)
        {
            review.CreatedAt = DateTime.Now;
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();
        }
    }
}
