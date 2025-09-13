using Feed_Bridge.Models.Entities;

namespace Feed_Bridge.IServices
{
    public interface IOrderService
    {
        Task<(bool Success, string Message, Order Order)> ConfirmOrderAsync(string userId);
        Task<Order?> GetOrderByIdAsync(int orderId);
        Task<IEnumerable<Order>> GetUserOrdersAsync(string userId);
    }
}
