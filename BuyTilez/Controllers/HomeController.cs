using BuyTilez.Data.Data.Repository.IRepository;
using BuyTilez.Models;
using BuyTilez.Models.ViewModels;
using BuyTilez.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace BuyTilez.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductRepository _productRepo;
        private readonly ICategoryRepository _categoryRepo;
        private readonly ICartRepository _cartRepo;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(ILogger<HomeController> logger, IProductRepository productRepository, ICategoryRepository categoryRepo, ICartRepository cartRepository, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _productRepo = productRepository;
            _categoryRepo = categoryRepo;
            _cartRepo = cartRepository;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            HomeViewModel homeViewModel = new HomeViewModel()
            {
                Products = _productRepo.GetAll(includeProperties: "Category,ApplicationType"),
                Categories = _categoryRepo.GetAll()
            };

            return View(homeViewModel);
        }

        public IActionResult Detail(int Id)
        {
            List<ShoppingCart> cartList = new List<ShoppingCart>();
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            if (claimsIdentity.IsAuthenticated)
            {
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                var userId = claim.Value;
                cartList = _cartRepo.GetUserCart(userId).ToList();
            }
            DetailViewModel detailViewModel = new DetailViewModel()
            {
                Product = _productRepo.GetFirstOrDefault(p => p.Id == Id, includeProperties: "Category,ApplicationType"),
                ExistsInCart = false
            };

            foreach (var item in cartList)
            {
                if (item.ProductId == Id)
                {
                    detailViewModel.ExistsInCart = true;
                }
            }

            return View(detailViewModel);
        }

        [Authorize]
        [HttpPost, ActionName("Detail")]
        public IActionResult DetailPost(int Id, DetailViewModel detailViewModel)
        {
            List<ShoppingCart> cartList = new List<ShoppingCart>();
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var userId = claim.Value != null? claim.Value:"-1";
            _cartRepo.Add(new ShoppingCart { ProductId = Id, SquareMeters = detailViewModel.Product.TempSquareMeters, UserId = userId });
            _cartRepo.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult RemoveFromCart(int Id)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var userId = claim.Value;
            List<ShoppingCart> cartList = _cartRepo.GetUserCart(userId).ToList();
            var productToRemove = cartList.SingleOrDefault(x => x.ProductId == Id);
            if (productToRemove != null)
            {
                cartList.Remove(productToRemove);
            }
            _cartRepo.Update(cartList.SingleOrDefault());
            _cartRepo.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
