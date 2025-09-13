using Feed_Bridge.Models.Entities;

namespace Feed_Bridge.IServices
{
    public interface IDonationService
    {
        Task Add(Donation donation, string userId);
        Task<IEnumerable<Donation>> GetAllDonations();
        Task<IEnumerable<Donation>> GetAllAcceptedDonations();
        Task<Donation> GetDonationById(int id);
        Task<decimal> GetTotalDonationsAmount();
        Task DeleteDonation(int id);
        Task UpdateDonation(Donation donation);


    }
}
