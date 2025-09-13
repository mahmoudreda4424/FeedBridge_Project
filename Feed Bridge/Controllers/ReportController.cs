using Feed_Bridge.IServices;
using Feed_Bridge.Models.Entities;
using Feed_Bridge.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Feed_Bridge.Controllers
{
    public class ReportController : Controller
    {
        private readonly IReportService _reportService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;

        public ReportController(IReportService reportService, UserManager<ApplicationUser> userManager,
            IEmailService emailService)
        {
            _reportService = reportService;
            _userManager = userManager;
            _emailService = emailService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var allReports = await _reportService.GetAll();
            return View(allReports);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var donors = await _userManager.Users
        .Where(u => u.Supports.Any() ||  u.Donnations.Any()) // اللي تبرعوا بس
        .ToListAsync();
            
            var vm = new ReportViewModel
            {
                Donors = donors
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ReportViewModel model)
        {
            if (ModelState.IsValid)
            {
                var donor = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == model.SelectedUserId);
                if (donor == null)
                {
                    ModelState.AddModelError("", "المتبرع غير موجود");
                    return View(model);
                }

                var report = new Report
                {
                    title = model.Title,
                    content = model.Content,
                    Email = donor.Email, // ناخده أوتوماتيك
                    CreatedAt = DateTime.Now,
                    UserId = _userManager.GetUserId(User) // الأدمن اللي أنشأ التقرير
                };

                await _reportService.Create(report);
                var emailBody = $@"
<!DOCTYPE html>
<html lang='ar' dir='rtl'>
<head>
    <meta charset='UTF-8'>
    <style>
        body {{
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            background-color: #f9f9f9;
            color: #333;
            padding: 20px;
        }}
        .container {{
            max-width: 600px;
            margin: auto;
            background: #fff;
            border-radius: 8px;
            overflow: hidden;
            box-shadow: 0 0 10px rgba(0,0,0,0.1);
        }}
        .header {{
            background: #4CAF50;
            color: white;
            padding: 15px;
            text-align: center;
            font-size: 20px;
        }}
        .content {{
            padding: 20px;
            line-height: 1.6;
        }}
        .footer {{
            background: #f1f1f1;
            text-align: center;
            padding: 10px;
            font-size: 12px;
            color: #777;
        }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            تقرير جديد من فريق FeedBridge
        </div>
        <div class='content'>
            <h2>{model.Title}</h2>
            <p>{model.Content}</p>
            <p>📅 <strong>تاريخ الإنشاء:</strong> {DateTime.Now:dd/MM/yyyy HH:mm}</p>
        </div>
        <div class='footer'>
            &copy; {DateTime.Now.Year} FeedBridge - جميع الحقوق محفوظة
        </div>
    </div>
</body>
</html>";
                // إرسال الإيميل للمتبرع
                await _emailService.SendEmailAsync(
                    donor.Email,
                    model.Title,
                    emailBody
                );

                TempData["SuccessMessage"] = "تم إرسال التقرير للمتبرع بنجاح";
                return RedirectToAction("Reports", "Admin");
            }
            ModelState.AddModelError("", "Error");
            // لو فيه مشكلة نرجع القائمة تاني
            model.Donors = await _userManager.Users
                .Where(u => u.Supports.Any())
                .ToListAsync();

            return View(model);
        }
    }
}
