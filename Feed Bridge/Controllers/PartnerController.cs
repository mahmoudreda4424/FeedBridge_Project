using Feed_Bridge.IServices;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Feed_Bridge.Controllers
{
    public class PartnerController : Controller
    {
         private readonly IParteerService _partenerService;
        public PartnerController(IParteerService partenerService)
        {
                        _partenerService = partenerService;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(IFormFile imageUrl, string redirectUrl,string Name)
        {
            if(Name == null)
            {
                ModelState.AddModelError("Name", "Name is required.");
                return View();
            }
            if (imageUrl == null || imageUrl.Length == 0)
            {
                ModelState.AddModelError("ImageUrl", "Image is required.");
                return View();
            }
            // Validate file type (optional)
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var extension = Path.GetExtension(imageUrl.FileName).ToLower();
            if (!allowedExtensions.Contains(extension))
            {
                ModelState.AddModelError("ImageUrl", "Invalid image format. Allowed formats: .jpg, .jpeg, .png, .gif");
                return View();
            }
            // Save the file to a directory (e.g., wwwroot/images/partners)
            var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "partners");
            if (!Directory.Exists(uploadsDir))
            {
                Directory.CreateDirectory(uploadsDir);
            }
            var uniqueFileName = Guid.NewGuid().ToString() + extension;
            var filePath = Path.Combine(uploadsDir, uniqueFileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageUrl.CopyToAsync(stream);
            }
            // Create a new Partener entity
            var partener = new Models.Entities.Partener
            {
                ImageUrl = "~/uploads/partners/" + uniqueFileName,
                RedirectUrl = redirectUrl,
                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier), // Assuming User.Identity.Name contains the user ID
                Name = Name
            };
            // Save to database using your service (inject IParteerService in constructor)
             await _partenerService.Create(partener);
            return RedirectToAction("AllPartners","Admin");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            await _partenerService.Delete(id);

            return RedirectToAction("AllPartners", "Admin");
        }
    

}
}
