using Feed_Bridge.IServices;
using Feed_Bridge.Models.Data;
using Feed_Bridge.Models.Entities;
using Feed_Bridge.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Feed_Bridge.Controllers
{
    public class SupportsController : Controller
    {
        private readonly ISupportService _supportService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly INotificationService _notificationService;


        public SupportsController(ISupportService supportService, UserManager<ApplicationUser> userManager, INotificationService notificationService)
        {
            _supportService = supportService;
            _userManager = userManager;
            _notificationService = notificationService;
        }

        // GET: Supports/Donate
        [Authorize]
        [HttpGet]
        public IActionResult Donate()
        {
            return View();
        }

        // POST: Supports/Donate
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Donate(string cardNumber, string cardholderName, decimal donationAmount)
        {
            if (donationAmount <= 0 || string.IsNullOrWhiteSpace(cardNumber) || string.IsNullOrWhiteSpace(cardholderName))
            {
                ModelState.AddModelError("", "الرجاء إدخال جميع بيانات الدفع بشكل صحيح");
                return View();
            }

            var userId = _userManager.GetUserId(User);

            var support = new Support
            {
                Amount = donationAmount,
                PaymentDate = DateTime.Now,
                PaymentMethod = "Card", 
                TransactionId = Guid.NewGuid().ToString(),
                Status = PaymentStatus.Success,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                UserId = userId
            };

            await _supportService.AddSupportAsync(support);
            var user = await _userManager.FindByIdAsync(userId);
            var admins = await _userManager.GetUsersInRoleAsync("Admin");
            foreach (var admin in admins)
            {
                await _notificationService.AddNotificationAsync(new Notification
                {
                    Title = "دعم مالي جديد",
                    Description = $"{user.UserName} قدم دعم مالي بقيمة {support.Amount} جنيه",
                    RedirectUrl = Url.Action("GetAllSupports", "Admin"),
                    UserId = admin.Id
                });
            }
            return View("Success");
        }
    }
}
