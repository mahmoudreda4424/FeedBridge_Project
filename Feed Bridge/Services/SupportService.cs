using Feed_Bridge.IServices;
using Feed_Bridge.Models.Data;
using Feed_Bridge.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Feed_Bridge.Services
{
    public class SupportService : ISupportService
    {
        private readonly AppDbContext _context;

        public SupportService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Support> AddSupportAsync(Support support)
        {
            _context.Supports.Add(support);
            await _context.SaveChangesAsync();
            return support;
        }

        public async Task<Support?> GetSupportByIdAsync(int id)
        {
            return await _context.Supports
                                 .Include(s => s.User)
                                 .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<IEnumerable<Support>> GetAllSupportsAsync()
        {
            return await _context.Supports
                                 .Include(s => s.User)
                                 .ToListAsync();
        }
    }
}
