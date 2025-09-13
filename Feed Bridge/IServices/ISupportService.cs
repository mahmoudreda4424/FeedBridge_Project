using Feed_Bridge.Models.Entities;

namespace Feed_Bridge.IServices
{
    public interface ISupportService
    {
        Task<Support> AddSupportAsync(Support support);
        Task<Support?> GetSupportByIdAsync(int id);
        Task<IEnumerable<Support>> GetAllSupportsAsync();
    }
}
