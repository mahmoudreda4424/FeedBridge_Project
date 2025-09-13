using Feed_Bridge.IServices;
using Feed_Bridge.Models.Data;
using Feed_Bridge.Models.Entities;
using Feed_Bridge.Models.Enums;
using Feed_Bridge.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Feed_Bridge.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IDonationService _donationService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IStaticPageService _staticPageService;


        public AdminController(AppDbContext context,IDonationService donationService, 
            UserManager<ApplicationUser> userManager, IStaticPageService staticPageService)
        {
            _context = context;
            _donationService = donationService;
            _userManager = userManager;
            _staticPageService = staticPageService;
        }

        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            //// إجمالي المتبرعين (عدد المستخدمين اللي عندهم تبرعات)
            var totalDonors = await _context.Donations
                .Select(d => d.UserId)
                .Distinct()
                .CountAsync();

            // إجمالي التبرعات (عدد التبرعات)
            var totalDonations = await _context.Donations.CountAsync();

            // إجمالي المساعدات المالية (نجمع قيمة التبرعات المالية)
            var totalSupports = await _context.Supports.SumAsync(s => (decimal?)s.Amount) ?? 0;

            // نحط الأرقام في ViewData أو ViewModel
            ViewData["TotalDonors"] = totalDonors;
            ViewData["TotalDonations"] = totalDonations;
            ViewData["TotalSupports"] = totalSupports;
            ViewData["ActivePage"] = "Dashboard";


            return View();
        }

        // Orders
        [HttpGet]
        public async Task<IActionResult> Orders()
        {
            var orders = await _context.Orders
                .Include(o => o.User) // لو عايز تعرض بيانات المستخدم
                .Include(o => o.OrderProducts)
                    .ThenInclude(op => op.Product)
                    .Where(x => x.Status == OrderStatus.Pending)
                .ToListAsync();

            return View(orders);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Approve(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return NotFound();

            order.Status = OrderStatus.Approved;
            await _context.SaveChangesAsync();

            // TODO: ابعت Notification / Email للمستخدم
            return RedirectToAction("Orders");
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Reject(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return NotFound();

            order.Status = OrderStatus.Rejected;
            await _context.SaveChangesAsync();

            // TODO: ابعت Notification / Email للمستخدم
            return RedirectToAction("Orders");
        }

        [HttpGet] 
        public async Task<IActionResult> Donate()
        {
            ViewData["ActivePage"] = "Donors";
            var donations = await _donationService.GetAllDonations();
            return View(donations);
        } 

        [HttpGet]
        public async Task<IActionResult> GetAllSupports()
        {
            var supports = await _context.Supports
                .Include(s => s.User)
                .ToListAsync();
            return View(supports);
        }

        [HttpGet]
        public async Task<IActionResult> Reports()
        {
            var reports = await _context.Reports.ToListAsync();
            return View(reports);
        }

        [HttpGet]
        public async Task<IActionResult> Products()
        {
            var products = await _context.Products.ToListAsync();
            return View(products);
        }

        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == id);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return RedirectToAction("products");
        }

        [HttpGet]
        public async Task<IActionResult> AllUsers()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            var users = await _userManager.Users
                // && !u.IsDeleted <= لو مش عاوزه يظهر المستخدمين اللي محذوفين اكتب 
                .Where(u => u.Id != currentUser.Id) 
                .ToListAsync();

            var model = new List<UserWithRoleVM>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                model.Add(new UserWithRoleVM
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    ImgUrl = user.ImgUrl,
                    Roles = roles,
                    IsFrozen = user.IsFrozen, // ✅ ربط الحالة من قاعدة البيانات
                    IsDeleted = user.IsDeleted, // ✅ ربط الحالة من قاعدة البيانات
                    DeletedBy = user.DeletedBy // ✅ ربط الحالة من قاعدة البيانات
                });
            }

            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> DeletedUsers()
        {
            var users = await _userManager.Users
                .Where(u => u.IsDeleted) 
                .ToListAsync();

            var model = new List<UserWithRoleVM>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                model.Add(new UserWithRoleVM
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    ImgUrl = user.ImgUrl,
                    Roles = roles,
                    IsFrozen = user.IsFrozen,
                    IsDeleted = user.IsDeleted,
                    DeletedBy = user.DeletedBy
                });
            }

            return View(model); // اعمل View بنفس شكل AllUsers أو نسخة خاصة للمحذوفين
        }


        [HttpGet]
        public async Task<IActionResult> AllPartners()
        {
            
            var partners = await _context.Parteners.ToListAsync();
            return View(partners);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeUserRole(string userId, string newRole)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();

            var roles = await _userManager.GetRolesAsync(user);

            // Remove old roles
            await _userManager.RemoveFromRolesAsync(user, roles);

            // Add new role
            await _userManager.AddToRoleAsync(user, newRole);

            TempData["Success"] = $"تم تحديث دور المستخدم {user.UserName} بنجاح إلى {newRole}.";
            return RedirectToAction("AllUsers");
        }

        [HttpGet]
        public async Task<IActionResult> EditHome()
        {
            var content = await _staticPageService.GetContent() ?? new StaticPage();
            return View(content);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditHome(StaticPage model, IFormFile? VideoFile)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (VideoFile != null && VideoFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/videos");

                // لو المجلد مش موجود ينشئه
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var fileName = Path.GetFileName(VideoFile.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await VideoFile.CopyToAsync(stream);
                }

                model.VideoUrl = "/uploads/videos/" + fileName; // حفظ رابط الفيديو في الداتا بيز
            }

            model.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            await _staticPageService.UpdateContent(model);

            return RedirectToAction("Index", "Home");
        }

        
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken] 
        public async Task<IActionResult> ToggleFreeze(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Json(new { success = false, message = "المستخدم غير موجود" });
            }

            user.IsFrozen = !user.IsFrozen;
            await _userManager.UpdateAsync(user);

            await _userManager.UpdateSecurityStampAsync(user);

            return Json(new
            {
                success = true,
                isFrozen = user.IsFrozen,
                message = user.IsFrozen ? "تم تجميد الحساب بنجاح" : "تم إلغاء التجميد بنجاح"
            });
        }

            
        [HttpPost]
        [ValidateAntiForgeryToken] 
        public async Task<IActionResult> MarkNotificationAsRead(int id)
        {
            var notif = await _context.Notifications.FindAsync(id);
            if (notif == null)
                return NotFound();

            notif.IsRead = true;
            await _context.SaveChangesAsync();

            return Ok();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            user.Email = user.Email + ".deleted." + Guid.NewGuid();
            user.NormalizedEmail = user.Email.ToUpper();
            user.UserName = user.UserName + ".deleted." + Guid.NewGuid();
            user.NormalizedUserName = user.UserName.ToUpper();
            user.IsDeleted = true;
            user.DeletedBy = "Admin";

            await _userManager.UpdateSecurityStampAsync(user);

            await _userManager.UpdateAsync(user);

            return RedirectToAction("AllUsers");
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> TrackDeliveries()
        {
            var orders = await _context.Orders
                .Include(o => o.Delivery)
                .Where(x => x.Status == OrderStatus.Delivered || x.Status == OrderStatus.Assigned || x.Status == OrderStatus.Approved) // الطلبات اللي ليها دليفري
                .OrderByDescending(o => o.UpdatedAt)
                .ToListAsync();

            ViewData["ActivePage"] = "TrackDelivery";
            return View(orders);
        }
    }
}
