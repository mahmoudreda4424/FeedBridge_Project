using Feed_Bridge.Models.Entities;
using Feed_Bridge.Services;
using Feed_Bridge.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Mail;

namespace Feed_Bridge.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _config;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration config)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _config = config;
        }

        // ---------------- Login ----------------
        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user != null && !user.IsDeleted)
            {
                // تحقق هل الحساب مجمد
                if (user.IsFrozen)
                {
                    ViewBag.Error = "حسابك مجمد من قبل الإدارة. برجاء التواصل مع الدعم.";
                    return View();
                }

                var result = await _signInManager.PasswordSignInAsync(
                    user, password, isPersistent: false, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    var roles = await _userManager.GetRolesAsync(user);

                    if (roles.Contains("Admin"))
                        return RedirectToAction("Dashboard", "Admin");
                    else if (roles.Contains("Delivery"))
                        return RedirectToAction("Dashboard", "Delivery");
                    else
                        return RedirectToAction("Index", "Home");
                }
            }

            ViewBag.Error = "البريد الإلكتروني أو كلمة المرور غير صحيحة";
            return View();
        }

        // ---------------- Register ----------------
        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError("Email", "هذا البريد الإلكتروني مستخدم بالفعل");
                return View(model);
            }

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.UserName.Replace(" ", "_"),
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    BirthDate = model.BirthDate,
                    Address = model.Address

                };

                // رفع صورة البروفايل
                if (model.ImgFile != null && model.ImgFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "profiles");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    var uniqueFileName = Guid.NewGuid() + Path.GetExtension(model.ImgFile.FileName);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                        await model.ImgFile.CopyToAsync(fileStream);

                    user.ImgUrl = "/uploads/profiles/" + uniqueFileName;
                }

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // أي مستخدم جديد بياخد Role "User" تلقائي
                    await _userManager.AddToRoleAsync(user, "User");

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }

        // ---------------- Logout ----------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        // ---------------- Forgot Password ----------------
        [HttpGet]
        public IActionResult ForgotPassword() => View();

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                ViewBag.Error = "من فضلك أدخل البريد الإلكتروني";
                return View();
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                ViewBag.Error = "هذا البريد غير مسجل لدينا";
                return View();
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var resetLink = Url.Action("ResetPassword", "Account",
                new { token, email = user.Email }, Request.Scheme);

            // إعداد الإيميل من appsettings.json
            var smtpEmail = _config["Smtp:Email"];
            var smtpPassword = _config["Smtp:Password"];

            var fromAddress = new MailAddress(smtpEmail, "FeedBridge Support");
            var toAddress = new MailAddress(user.Email);
            string subject = "إعادة تعيين كلمة المرور - FeedBridge";

            string body = $@"
            <!DOCTYPE html>
            <html lang='ar'>
            <head>
              <meta charset='UTF-8'>
              <meta name='viewport' content='width=device-width, initial-scale=1.0'>
              <style>
                body {{
                  font-family: 'Arial', sans-serif;
                  background-color: #f5f5f5;
                  padding: 20px;
                  direction: rtl;
                  text-align: center;
                  margin: 0;
                }}
                .container {{
                  max-width: 600px;
                  margin: 0 auto;
                  background-color: #ffffff;
                  border-radius: 12px;
                  box-shadow: 0 4px 8px rgba(0,0,0,0.1);
                  overflow: hidden;
                  text-align: center;
                }}
                .card-header {{
                  background-color: #1cba7f;
                  color: #fff;
                  padding: 30px;
                  border-top-left-radius: 12px;
                  border-top-right-radius: 12px;
                }}
                .card-header h2 {{ margin: 0; font-size: 24px; text-align: center; }}
                .card-body {{ padding: 30px; color: #333; line-height: 1.8; text-align: center; }}
                .btn-wrapper {{ padding: 0 30px 30px; text-align: center; }}
                .btn {{
                  display: inline-block;
                  padding: 12px 24px;
                  font-size: 16px;
                  color: #fff !important;
                  background-color: #1cba7f;
                  border-radius: 8px;
                  text-decoration: none;
                  transition: background-color 0.3s ease;
                }}
                .btn:hover {{ background-color: #169363; }}
                .disclaimer {{ margin-top: 20px; font-size: 12px; color: #777; text-align: center; }}
              </style>
            </head>
            <body>
              <div class='container'>
                <div class='card-header'>
                  <h2>إعادة تعيين كلمة المرور</h2>
                </div>
                <div class='card-body'>
                  <p>مرحباً <b>{user.UserName}</b>،</p>
                  <p>اضغط على الزر أدناه لإعادة تعيين كلمة المرور الخاصة بك:</p>
                  <div class='btn-wrapper'>
                    <a href='{resetLink}' class='btn'>إعادة التعيين الآن</a>
                  </div>
                  <p class='disclaimer'>إذا لم تطلب ذلك، تجاهل هذه الرسالة.</p>
                </div>
              </div>
            </body>
            </html>";

            using (var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, smtpPassword)
            })
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
            {
                smtp.Send(message);
            }

            ViewBag.Message = "تم إرسال رابط إعادة تعيين كلمة المرور إلى بريدك الإلكتروني.";
            return View();
        }

        // ---------------- Reset Password ----------------
        [HttpGet]
        public IActionResult ResetPassword(string token, string email)
        {
            if (token == null || email == null)
            {
                ViewBag.Error = "الرابط غير صالح";
                return RedirectToAction("Login");
            }

            var model = new ResetPasswordViewModel { Token = token, Email = email };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ViewBag.Error = "المستخدم غير موجود";
                return View();
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
            if (result.Succeeded)
            {
                ViewBag.Success = "تم تغيير كلمة المرور بنجاح";
                return RedirectToAction("Login");
            }

            ViewBag.Error = string.Join(", ", result.Errors.Select(e => e.Description));
            return View(model);
        }
    }
}