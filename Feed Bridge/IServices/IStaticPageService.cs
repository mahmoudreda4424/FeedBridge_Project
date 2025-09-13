using Feed_Bridge.Models.Entities;

namespace Feed_Bridge.IServices
{
    public interface IStaticPageService
    {
        Task<StaticPage> GetContent();
        Task UpdateContent(StaticPage staticPage);
    }
}
