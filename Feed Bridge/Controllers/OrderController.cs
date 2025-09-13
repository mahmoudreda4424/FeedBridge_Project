using Feed_Bridge.IServices;
using Feed_Bridge.Models.Entities;
using Feed_Bridge.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Feed_Bridge.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly INotificationService _notificationService;

        public OrderController(IOrderService orderService, UserManager<ApplicationUser> userManager, INotificationService notificationService)
        {
            _orderService = orderService;
            _userManager = userManager;
            _notificationService = notificationService;
        }
        [HttpPost]
        public async Task<IActionResult> Confirm()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var (success, message, order) = await _orderService.ConfirmOrderAsync(user.Id);

            TempData[success ? "Success" : "Error"] = message;

            if (!success)
                return RedirectToAction("Index", "Cart");
            var admins = await _userManager.GetUsersInRoleAsync("Admin");
            foreach (var admin in admins)
            {
                await _notificationService.AddNotificationAsync(new Notification
                {
                    Title = "طلب جديد",
                    Description = $"تم إنشاء طلب جديد رقم #{order.Id}",
                    RedirectUrl = Url.Action("Orders", "Admin"),
                    UserId = admin.Id
                });
            }
            var deliveries = await _userManager.GetUsersInRoleAsync("Delivery");
            foreach (var delivery in deliveries)
            {
                await _notificationService.AddNotificationAsync(new Notification
                {
                    Title = "طلب جديد للتوصيل",
                    Description = $"طلب رقم #{order.Id} جاهز للتوصيل",
                    RedirectUrl = Url.Action("Orders", "Delivery"),
                    UserId = delivery.Id
                });
            }

            return RedirectToAction("Details", "Order", new { id = order.Id });
        }

        public async Task<IActionResult> Details(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var order = await _orderService.GetOrderByIdAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            // لو المستخدم أدمن يقدر يشوف أي أوردر
            if (!User.IsInRole("Admin") && order.UserId != user.Id)
            {
                return NotFound();
            }

            return View(order);
        }

        public async Task<IActionResult> History(string? userId)
        {
            // لو الأدمن مرر userId من اللينك
            if (!string.IsNullOrEmpty(userId))
            {
                var ordersForUser = await _orderService.GetUserOrdersAsync(userId);
                return View(ordersForUser);
            }

            // لو مفيش userId في الراوت → نستخدم اليوزر الحالي (مستخدم عادي)
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return RedirectToAction("Login", "Account");

            var orders = await _orderService.GetUserOrdersAsync(currentUser.Id);
            return View(orders);
        }

    }
}
