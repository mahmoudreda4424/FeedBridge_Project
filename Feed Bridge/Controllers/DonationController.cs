using Feed_Bridge.IServices;
using Feed_Bridge.Models.Entities;
using Feed_Bridge.Models.Enums;
using Feed_Bridge.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Feed_Bridge.Controllers
{
    [Authorize]
    public class DonationController : Controller
    {
        private readonly IDonationService _donationService;
        private readonly IProductService _productService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;
        private readonly INotificationService _notificationService;

        public DonationController(
            IDonationService donationService,
            IProductService productService,
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment env,
            INotificationService notificationService)
        {
            _donationService = donationService;
            _productService = productService;
            _userManager = userManager;
            _env = env;
            _notificationService = notificationService;
        }

        // 🟢 عرض كل التبرعات (للأدمن فقط)
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            ViewData["ActivePage"] = "Donate";
            var donations = await _donationService.GetAllDonations();
            return View(donations);
        }


        // 🟢 إنشاء تبرع (من المستخدم)
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DonationViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            // رفع الصورة
            string? fileName = null;
            if (model.Image != null)
            {
                fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.Image.FileName);
                string path = Path.Combine(_env.WebRootPath, "uploads", fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await model.Image.CopyToAsync(stream);
                }
            }

            // إنشاء التبرع (في وضع Pending)
            var donation = new Donation
            {
                Name = model.Name,
                ImgURL = fileName,
                ExpirDate = model.ExpirDate,
                Quantity = model.Quantity,
                Address = model.Address,
                Phone = model.Phone,
                Description = model.Description,
                Category = model.Category,
                Status = DonationStatus.Pending, // ⬅️ حالة التبرع
            };
            await _donationService.Add(donation, user.Id);

            // إرسال إشعار للأدمن
            var admins = await _userManager.GetUsersInRoleAsync("Admin");
            foreach (var admin in admins)
            {
                await _notificationService.AddNotificationAsync(new Notification
                {
                    Title = "تبرع جديد",
                    Description = $"{user.UserName} تبرع بمنتج {donation.Name}",
                    RedirectUrl = Url.Action("Donate", "Admin"),
                    UserId = admin.Id
                });
            }

            TempData["SuccessMessage"] = "تم إرسال التبرع بنجاح وسيقوم الأدمن بمراجعته.";
            return RedirectToAction("Create");
        }

        // 🟢 قبول التبرع (Admin فقط)
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Accept(int id)
        {
            var donation = await _donationService.GetDonationById(id);
            if (donation == null) return NotFound();

            // ✅ لو التبرع مش في حالة Pending ما ينفعش يتقبل تاني
            if (donation.Status != DonationStatus.Pending)
            {
                TempData["ErrorMessage"] = "هذا التبرع تمت مراجعته بالفعل.";
                return RedirectToAction("Donate", "Admin");
            }

            donation.Status = DonationStatus.Accepted;
            await _donationService.UpdateDonation(donation);

            // ✅ قبل إضافة المنتج نتأكد إنه مش مضاف بالفعل
            var productExists = await _productService.GetByDonationId(donation.Id);
            if (productExists == null)
            {
                var product = new Product
                {
                    Name = donation.Name,
                    ImgURL = donation.ImgURL,
                    ExpirDate = donation.ExpirDate,
                    Quantity = donation.Quantity,
                    DonationId = donation.Id,
                    Category = donation.Category
                };
                await _productService.AddAsync(product);
            }

            // إشعار للمتبرع
            await _notificationService.AddNotificationAsync(new Notification
            {
                Title = "تم قبول التبرع",
                Description = $"تم قبول تبرعك ({donation.Name}) وإضافته إلى المنتجات.",
                RedirectUrl = Url.Action("Index", "Product"),
                UserId = donation.UserId
            });

            
            return RedirectToAction("Donate", "Admin");
        }

        // 🟢 رفض التبرع (Admin فقط)
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Reject(int id)
        {
            var donation = await _donationService.GetDonationById(id);
            if (donation == null) return NotFound();

            if (donation.Status != DonationStatus.Pending)
            {
                TempData["ErrorMessage"] = "هذا التبرع تمت مراجعته بالفعل.";
                return RedirectToAction("Donate", "Admin");
            }

            donation.Status = DonationStatus.Rejected;
            await _donationService.UpdateDonation(donation);

            await _notificationService.AddNotificationAsync(new Notification
            {
                Title = "تم رفض التبرع",
                Description = $"نأسف، تم رفض تبرعك ({donation.Name}).",
                RedirectUrl = Url.Action("Create", "Donation"),
                UserId = donation.UserId
            });

            TempData["ErrorMessage"] = " تم رفض التبرع.";
            return RedirectToAction("Donate", "Admin");
        }


        // 🟢 حذف التبرع (Admin فقط)
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            await _donationService.DeleteDonation(id);
            TempData["SuccessMessage"] = "تم حذف التبرع بنجاح";
            return RedirectToAction("Donate", "Admin");
        }

       
    }
}

