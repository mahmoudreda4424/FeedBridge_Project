using Feed_Bridge.IServices;
using Feed_Bridge.Models.Data;
using Feed_Bridge.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Feed_Bridge.Services
{
    public class StaticPageService:IStaticPageService
    {
        private readonly AppDbContext _context;

        public StaticPageService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<StaticPage> GetContent()
        {
            return await _context.StaticPages.FirstOrDefaultAsync();
        }

        public async Task UpdateContent(StaticPage content)
        {
            var existing = await _context.StaticPages.FirstOrDefaultAsync();
            if (existing != null)
            {
                existing.Content1 = content.Content1;
                existing.Content2 = content.Content2;
                existing.VideoUrl = content.VideoUrl;
                existing.PartenerBackgroundImageUrl = content.PartenerBackgroundImageUrl;
                existing.HomePageBackgroundImageUrl = content.HomePageBackgroundImageUrl;
                existing.UserId = content.UserId;
            }
            else
            {
                await _context.StaticPages.AddAsync(content);
            }
            await _context.SaveChangesAsync();
        }

    }
}
