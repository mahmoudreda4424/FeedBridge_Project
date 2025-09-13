using Feed_Bridge.IServices;
using Feed_Bridge.Models.Data;
using Feed_Bridge.Models.Entities;
using Feed_Bridge.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace Feed_Bridge.Services
{
    public class OrderService : IOrderService
    {

        private readonly AppDbContext _context;

        public OrderService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<(bool Success, string Message, Order Order)> ConfirmOrderAsync(string userId)
        {
            var cart = await _context.Carts
                .Include(c => c.ProductCarts)
                .ThenInclude(pc => pc.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || !cart.ProductCarts.Any())
            {
                return (false, "السلة فارغة!", null);
            }

            // إنشاء الطلب
            var order = new Order
            {
                UserId = userId,
                CartId = cart.Id,
                Status = OrderStatus.Pending,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                OrderProducts = cart.ProductCarts.Select(pc => new OrderProduct
                {
                    ProductId = pc.ProductId,
                    Quantity = pc.Quantity
                }).ToList()
            };

            // تحديث كميات المنتجات
            foreach (var pc in cart.ProductCarts)
            {
                var product = pc.Product;
                if (product.Quantity >= pc.Quantity)
                {
                    product.Quantity -= pc.Quantity;
                }
                else
                {
                    return (false, $" الكمية المطلوبة من {product.Name} غير متاحة.", null);
                }
            }

            // إضافة الطلب وحذف محتويات السلة
            _context.Orders.Add(order);
            _context.ProductCarts.RemoveRange(cart.ProductCarts);

            await _context.SaveChangesAsync();

            return (true, "تم تأكيد الطلب بنجاح!", order);
        }

        public async Task<Order?> GetOrderByIdAsync(int orderId)
        {
            return await _context.Orders
                .Include(o => o.OrderProducts)
                .ThenInclude(op => op.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId);
        }

        public async Task<IEnumerable<Order>> GetUserOrdersAsync(string userId)
        {
            return await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderProducts)
                .ThenInclude(op => op.Product)
                .ToListAsync();
        }
    }
}
