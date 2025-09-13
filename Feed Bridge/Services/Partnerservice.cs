using Feed_Bridge.IServices;
using Feed_Bridge.Models.Data;
using Feed_Bridge.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Feed_Bridge.Services
{
    public class Partnerservice: IParteerService
    {
        private readonly AppDbContext _context;
        public Partnerservice(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Partener>> GetAllAsync()
        {
            return await _context.Parteners.ToListAsync();
        }
        public async Task Create(Partener partener)
        {
            _context.Parteners.Add(partener);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var partener = await _context.Parteners.FindAsync(id);
            if (partener != null)
            {
                _context.Parteners.Remove(partener);
                await _context.SaveChangesAsync();
            }
        }
    }
}
