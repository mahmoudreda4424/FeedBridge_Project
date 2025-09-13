using Feed_Bridge.IServices;
using Feed_Bridge.Models.Entities;
using Feed_Bridge.Models.Enums;
using Feed_Bridge.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Feed_Bridge.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly IDonationService _donationService;

        public ProductController(IProductService productService, IDonationService donationService)
        {
            _productService = productService;
            _donationService = donationService;
        }

        // 🟢 عرض المنتجات (للجميع)
        public async Task<IActionResult> Index(string category)
        {
            var products = await _productService.GetAllAsync(category);
            var categories = Enum.GetNames(typeof(ProductCategory)).ToList();

            ViewBag.Categories = categories;
            ViewBag.SelectedCategory = category;

            return View(products);
        }

        // 🟢 إضافة منتج من التبرع (Admin فقط = قبول التبرع)
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddFromDonation(int donationId)
        {
            var donation = await _donationService.GetDonationById(donationId);
            if (donation == null)
                return NotFound();

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

            // ✅ تحديث حالة التبرع → تم القبول
            donation.Status = DonationStatus.Accepted;
            await _donationService.UpdateDonation(donation);

            TempData["SuccessMessage"] = " تمت إضافة التبرع إلى قائمة المنتجات";
            return RedirectToAction("GetAll", "Donation");
        }

        // 🟢 حذف منتج (Admin فقط)
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            await _productService.DeleteAsync(id);
            TempData["SuccessMessage"] = " تم حذف المنتج بنجاح";
            return RedirectToAction("Products", "Admin");
        }

        //[Authorize(Roles = "Admin")]
        //public async Task<IActionResult> Edit(int id)
        //{
        //    var product = await _productService.GetByIdAsync(id);
        //    if (product == null) return NotFound();

        //    return View(product);
        //}

        //[HttpPost]
        //[Authorize(Roles = "Admin")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, Product product)
        //{
        //    if (id != product.Id) return NotFound();

        //    if (ModelState.IsValid)
        //    {
        //        await _productService.UpdateAsync(product);
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(product);
        //}
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null) return NotFound();

            var donation = product.Donation;
            if (donation == null) return BadRequest("هذا المنتج غير مرتبط بتبرع");

            var viewModel = new EditProductViewModel
            {
                Id = product.Id,
                Name = product.Name,
                ExpirDate = product.ExpirDate,
                Quantity = product.Quantity,
                Address = donation.Address,
                Phone = donation.Phone,
                Description = donation.Description,
                ExistingImageUrl = product.ImgURL
            };

            return View(viewModel); // يفتح نفس الفيو Edit.cshtml
        }

        // -------- POST: Edit --------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditProductViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model); // لو فيه خطأ يرجع لنفس الفيو

            var product = await _productService.GetByIdAsync(model.Id);
            if (product == null) return NotFound();

            var donation = product.Donation;
            if (donation == null) return BadRequest("هذا المنتج غير مرتبط بتبرع");

            // تحديث المنتج
            product.Name = model.Name;
            product.ExpirDate = model.ExpirDate;
            product.Quantity = model.Quantity;
            


            // تحديث التبرع
            donation.Address = model.Address;
            donation.Phone = model.Phone;
            donation.Description = model.Description;
            product.Category = model.Category;

            // رفع صورة جديدة لو فيه
            if (model.Image != null)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = Guid.NewGuid() + Path.GetExtension(model.Image.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.Image.CopyToAsync(stream);
                }

                product.ImgURL = uniqueFileName;
            }

            // حفظ التعديلات
            await _productService.UpdateAsync(product);

            TempData["SuccessMessage"] = "تم تعديل المنتج بنجاح";

            return View(model);
        }
    }
}
