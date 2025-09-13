using Feed_Bridge.Models.Entities;

namespace Feed_Bridge.IServices
{
    public interface INotificationService
    {
        Task AddNotificationAsync(Notification notification);
        Task<List<Notification>> GetUserNotificationsAsync(string userId);
        Task<int> GetUnreadCountAsync(string userId);
        Task MarkAsReadAsync(int id);
    }
}
