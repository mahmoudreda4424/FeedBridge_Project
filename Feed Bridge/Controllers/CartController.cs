using Feed_Bridge.IServices;
using Feed_Bridge.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Feed_Bridge.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly UserManager<ApplicationUser> _userManager;

        public CartController(ICartService cartService, UserManager<ApplicationUser> userManager)
        {
            _cartService = cartService;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null) 
                return RedirectToAction("Login", "Account");

            var userCart = await _cartService.GetUserCart(user.Id);

            return View(userCart);
        }

        public async Task<IActionResult> Add(int productId, int quantity = 1)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _cartService.AddToCart(userId, productId, quantity);

            return Json(new { success = result.Success, message = result.Message });
        }

        //[HttpPost]
        //public async Task<IActionResult> IncreaseQuantity(int productId)
        //{
        //    var user = await _userManager.GetUserAsync(User);
        //    var (ok, msg) = await _cartService.IncreaseQuantity(user.Id, productId);

        //    return Json(new { success = ok, message = msg });
        //}

        //[HttpPost]
        //public async Task<IActionResult> DecreaseQuantity(int productId)
        //{
        //    var user = await _userManager.GetUserAsync(User);
        //    var (ok, msg) = await _cartService.DecreaseQuantity(user.Id, productId);

        //    return Json(new { success = ok, message = msg });
        //}

        [HttpPost]
        public async Task<IActionResult> Remove(int productId)
        {
            var user = await _userManager.GetUserAsync(User);
            var (ok, msg) = await _cartService.Remove(user.Id, productId);

            return Json(new { success = ok, message = msg });
        }

        [HttpPost]
        public async Task<IActionResult> IncreaseQuantity(int productId)
        {
            var user = await _userManager.GetUserAsync(User);
            var (ok, msg) = await _cartService.IncreaseQuantity(user.Id, productId);
            return Json(new { success = ok, message = msg });
        }

        [HttpPost]
        public async Task<IActionResult> DecreaseQuantity(int productId)
        {
            var user = await _userManager.GetUserAsync(User);
            var (ok, msg) = await _cartService.DecreaseQuantity(user.Id, productId);
            return Json(new { success = ok, message = msg });
        }
    }
}
