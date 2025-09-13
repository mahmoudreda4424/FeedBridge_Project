using Feed_Bridge.IServices;
using Feed_Bridge.Models.Data;
using Feed_Bridge.Models.Entities;
using Feed_Bridge.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Feed_Bridge.Controllers
{
    [Authorize(Roles = "Delivery")]
    public class DeliveryController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IDonationService _donationService;
        private readonly UserManager<ApplicationUser> _userManager;

        public DeliveryController(AppDbContext context, IDonationService donationService, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _donationService = donationService;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            var totalOrders = await _context.Orders
                .Where(o => o.Status == OrderStatus.Approved)
                .CountAsync();

            var completedOrders = await _context.Orders
                .Where(o => o.Status == OrderStatus.Delivered)
                .CountAsync();

            var totalDonations = await _donationService.GetAllDonations();

            ViewData["TotalOrders"] = totalOrders;
            ViewData["CompletedOrders"] = completedOrders;
            ViewData["TotalDonations"] = totalDonations.Count();

            ViewData["ActivePage"] = "Dashboard";
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Orders()
        {
            var orders = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderProducts)
                    .ThenInclude(op => op.Product)
                    // هيتشال من الصفحه order هيظهر بس الطلبات اللي متوافق عليها من الادمن لو حد من الدليفري اخد ال  
                    .Where(x => x.Status == OrderStatus.Approved)
                .ToListAsync();

            ViewData["ActivePage"] = "Orders";
            return View(orders);
        }

        // صفحة التبرعات للعرض
        [HttpGet]
        public async Task<IActionResult> Donations()
        {
            var donations = await _donationService.GetAllAcceptedDonations();
            ViewData["ActivePage"] = "Donations";
            return View(donations); 
        }

        [Authorize(Roles = "Delivery")]
        [HttpPost]
        public async Task<IActionResult> TakeOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return NotFound();

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            order.DeliveryId = currentUserId;   // حفظ مين الدليفري
            order.Status = OrderStatus.Assigned;
            order.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return RedirectToAction("Orders");
        }

        [Authorize(Roles = "Delivery")]
        [HttpPost]
        public async Task<IActionResult> TakeDonation(int id)
        {
            var donation = await _context.Donations.FindAsync(id);
            if (donation == null) return NotFound();

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            donation.DeliveryId = currentUserId;   // حفظ مين الدليفري
            donation.Status = DonationStatus.Assigned;
            donation.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return RedirectToAction("Donations");
        }

        [Authorize(Roles = "Delivery")]
        public async Task<IActionResult> MarkDelivered(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null || order.Status != OrderStatus.Assigned)
                return BadRequest("الطلب غير صالح للتوصيل");

            order.Status = OrderStatus.Delivered;
            await _context.SaveChangesAsync();

            return RedirectToAction("MyOrders");
        }

        [Authorize(Roles = "Delivery")]
        public async Task<IActionResult> MyOrders()
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var myOrders = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderProducts).ThenInclude(op => op.Product)
                .Where(o => o.DeliveryId == currentUserId &&
                            (o.Status == OrderStatus.Assigned || o.Status == OrderStatus.Delivered))
                .ToListAsync();

            ViewData["ActivePage"] = "Deliveries";
            return View(myOrders);
        }

        [Authorize(Roles = "Delivery")]
        public async Task<IActionResult> MyDonations()
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var donations = await _context.Donations
                .Include(d => d.User)
                .Include(d => d.Product)
                .Where(d => d.DeliveryId == currentUserId
                            && (d.Status == DonationStatus.Assigned || d.Status == DonationStatus.Delivered))
                .ToListAsync();

            ViewData["ActivePage"] = "Donations";
            return View(donations);
        }

        [Authorize(Roles = "Delivery")]
        public async Task<IActionResult> MarkAsTaken(int id)
        {
            var donation = await _context.Donations.FindAsync(id);
            if (donation == null)
                return BadRequest("التبرع غير صالح للاستلام");

            donation.Status = DonationStatus.Delivered;
            await _context.SaveChangesAsync();

            return RedirectToAction("MyDonations");
        }
    }
}
