using Feed_Bridge.Models.Entities;

namespace Feed_Bridge.IServices
{
    public interface IParteerService
    {
        Task<IEnumerable<Partener>> GetAllAsync();
        Task Create(Partener partener);
        Task Delete(int id);
    }
}
