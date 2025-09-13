using Feed_Bridge.IServices;
using Feed_Bridge.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Feed_Bridge.Views.Shared.Components
{
    public class NotificationsViewComponent :ViewComponent
    {
        private readonly INotificationService _notificationService;
        private readonly UserManager<ApplicationUser> _userManager;


        public NotificationsViewComponent(INotificationService notificationService, UserManager<ApplicationUser> userManager)
        {
            _notificationService = notificationService;
            _userManager = userManager;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = await _userManager.GetUserAsync((ClaimsPrincipal)User);
            var notifications = await _notificationService.GetUserNotificationsAsync(user.Id);
            return View(notifications);
        }
    }
}
