using Feed_Bridge.Models.Entities;

namespace Feed_Bridge.IServices
{
    public interface ICartService
    {
        public Task<(bool Success, string Message)> AddToCart(string userId, int productId, int quantity);

        Task<Cart> GetUserCart(string userId);

        public Task<(bool ok, string message)> IncreaseQuantity(string userId, int productId);

        public Task<(bool ok, string message)> DecreaseQuantity(string userId, int productId);

        public Task<(bool ok, string message)> Remove(string userId, int productId);
        //Task ClearCart(string userId);
    }
}
