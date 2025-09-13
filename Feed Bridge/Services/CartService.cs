using Feed_Bridge.IServices;
using Feed_Bridge.Models.Data;
using Feed_Bridge.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Feed_Bridge.Services
{
    public class CartService : ICartService
    {
        private readonly AppDbContext _context;

        public CartService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<(bool Success, string Message)> AddToCart(string userId, int productId, int quantity)
        {
            // 1. جلب المنتج
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
                return (false, "المنتج غير موجود.");

            // 2. التحقق من الكمية المتاحة
            if (product.Quantity < quantity)
                return (false, "الكمية المطلوبة غير متوفرة.");

            // 3. جلب السلة الخاصة باليوزر
            var cart = await _context.Carts
                .Include(c => c.ProductCarts)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId,
                    ProductCarts = new List<ProductCart>()
                };
                await _context.Carts.AddAsync(cart);
            }

            // 4. التحقق من وجود المنتج في السلة
            var existingProduct = cart.ProductCarts.FirstOrDefault(pc => pc.ProductId == productId);

            if (existingProduct != null)
            {
                // لو المنتج موجود بالفعل نزود الكمية
                if (existingProduct.Quantity + quantity > product.Quantity)
                    return (false, "لا يمكن إضافة هذه الكمية، المخزون غير كافٍ.");

                existingProduct.Quantity += quantity;
            }
            else
            {
                // لو مش موجود نضيفه
                cart.ProductCarts.Add(new ProductCart
                {
                    ProductId = productId,
                    Quantity = quantity
                });
            }

            await _context.SaveChangesAsync();
            return (true, "تمت إضافة المنتج إلى السلة بنجاح.");
        }

        public async Task<Cart> GetUserCart(string userId)
        {
            return await _context.Carts
                .Include(c => c.ProductCarts)
                .ThenInclude(x => x.Product) // Cartعشان لو احتاجتك معلومات عن المنتج وانا بظهره في ال 
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        public async Task<(bool ok, string message)> IncreaseQuantity(string userId, int productId)
        {
            var cart = await GetUserCart(userId);
            if (cart == null) return (false, "لا توجد سلة");

            var item = cart.ProductCarts.FirstOrDefault(pc => pc.ProductId == productId);
            if (item == null) return (false, "المنتج غير موجود في السلة");

            var product = item.Product ?? await _context.Products.FindAsync(productId);
            if (product == null) return (false, "المنتج غير موجود");

            if (item.Quantity + 1 > product.Quantity)
                return (false, $"لا يمكن الزيادة. المتاح: {product.Quantity}");

            item.Quantity += 1;
            await _context.SaveChangesAsync();
            return (true, "تم تعديل الكمية");
        }

        public async Task<(bool ok, string message)> DecreaseQuantity(string userId, int productId)
        {
            var cart = await GetUserCart(userId);
            if (cart == null) return (false, "لا توجد سلة");

            var item = cart.ProductCarts.FirstOrDefault(pc => pc.ProductId == productId);
            if (item == null) return (false, "المنتج غير موجود في السلة");

            if (item.Quantity <= 1)
                cart.ProductCarts.Remove(item);
            else
                item.Quantity -= 1;

            await _context.SaveChangesAsync();
            return (true, "تم تعديل الكمية");
        }

        public async Task<(bool ok, string message)> Remove(string userId, int productId)
        {
            var cart = await GetUserCart(userId);
            if (cart == null) return (false, "لا توجد سلة");

            var item = cart.ProductCarts.FirstOrDefault(pc => pc.ProductId == productId);
            if (item == null) return (false, "المنتج غير موجود في السلة");

            cart.ProductCarts.Remove(item);
            await _context.SaveChangesAsync();
            return (true, "تمت إزالة المنتج من السلة");
        }


        //public async Task ClearCart(string userId)
        //{
        //    var cartItems = _context.Carts.Where(c => c.UserId == userId);
        //    _context.Carts.RemoveRange(cartItems);
        //    await _context.SaveChangesAsync();
        //}
    }
}
