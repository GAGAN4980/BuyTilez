using BuyTilez.Data.Data.Repository.IRepository;
using BuyTilez.Models;
using BuyTilez.Models.ViewModels;
using BuyTilez.Utilities;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BuyTilez.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductRepository _productRepo;
        private readonly ICategoryRepository _categoryRepo;

        public HomeController(ILogger<HomeController> logger, IProductRepository productRepository, ICategoryRepository categoryRepo)
        {
            _logger = logger;
            _productRepo = productRepository;
            _categoryRepo = categoryRepo;
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
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(Constants.ShoppingCartSession) != null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(Constants.ShoppingCartSession).Count() > 0)
            {
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(Constants.ShoppingCartSession);
            }

            DetailViewModel detailViewModel = new DetailViewModel()
            {
                Product = _productRepo.GetFirstOrDefault(p => p.Id == Id, includeProperties: "Category,ApplicationType"),
                ExistsInCart = false
            };

            foreach (var item in shoppingCartList)
            {
                if (item.ProductId == Id)
                {
                    detailViewModel.ExistsInCart = true;
                }
            }

            return View(detailViewModel);
        }

        [HttpPost, ActionName("Detail")]
        public IActionResult DetailPost(int Id, DetailViewModel detailViewModel)
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(Constants.ShoppingCartSession) != null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(Constants.ShoppingCartSession).Count() > 0)
            {
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(Constants.ShoppingCartSession);
            }

            shoppingCartList.Add(new ShoppingCart { ProductId = Id, SquareMeters = detailViewModel.Product.TempSquareMeters });
            HttpContext.Session.Set(Constants.ShoppingCartSession, shoppingCartList);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult RemoveFromCart(int Id)
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(Constants.ShoppingCartSession) != null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(Constants.ShoppingCartSession).Count() > 0)
            {
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(Constants.ShoppingCartSession);
            }

            var productToRemove = shoppingCartList.SingleOrDefault(x => x.ProductId == Id);
            if (productToRemove != null)
            {
                shoppingCartList.Remove(productToRemove);
            }

            HttpContext.Session.Set(Constants.ShoppingCartSession, shoppingCartList);

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
