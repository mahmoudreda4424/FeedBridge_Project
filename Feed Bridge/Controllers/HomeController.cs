using Feed_Bridge.IServices;
using Feed_Bridge.Models;
using Feed_Bridge.Models.Entities;
using Feed_Bridge.Services;
using Feed_Bridge.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace Feed_Bridge.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IParteerService _partnerService;
        private readonly IStaticPageService _staticPageService;




        private readonly IReviewService _reviewService;

        public HomeController(ILogger<HomeController> logger, IReviewService reviewService, IParteerService partnerService, IStaticPageService staticPageService)
        {
            _logger = logger;
            _reviewService = reviewService;
            _partnerService = partnerService;
            _staticPageService = staticPageService;
        }

        

        public async Task<IActionResult> Index()
        {
            var reviews = await _reviewService.GetAllAsync();
            var partners = await _partnerService.GetAllAsync();
            var staticpage = await _staticPageService.GetContent(); // جلب محتوى الادمن


            var vm = new HomeViewModel
            {
                Reviews = reviews,
                Partners = partners,
                Content1 = staticpage?.Content1,
                Content2 = staticpage?.Content2,
                VideoUrl = staticpage?.VideoUrl
            };

            return View(vm);
        }
       
        [HttpPost]
        [Authorize]
       
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddReview(string FeedBackMsg, int StarsNumber)
        {
            var review = new Review
            {
                FeedBackMsg = FeedBackMsg,
                StarsNumber = StarsNumber,
                UserID = User.FindFirstValue(ClaimTypes.NameIdentifier),
                CreatedAt = DateTime.Now
            };
            await _reviewService.AddAsync(review);
            return RedirectToAction("Index");
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
