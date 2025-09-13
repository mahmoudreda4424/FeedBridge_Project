using Feed_Bridge.Models.Entities;

namespace Feed_Bridge.IServices
{
    public interface IReportService
    {
        Task<List<Report>> GetAll();
        Task Create(Report report);
    }
}
