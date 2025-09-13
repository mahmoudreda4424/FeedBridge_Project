using Feed_Bridge.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Feed_Bridge.IServices
{
    public interface IReviewService
    {
        Task<List<Review>> GetAllAsync();
        Task AddAsync(Review review);
    }
}



