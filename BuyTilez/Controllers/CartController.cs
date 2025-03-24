using Braintree;
using BuyTilez.Data.Data.Repository.IRepository;
using BuyTilez.Models;
using BuyTilez.Models.ViewModels;
using BuyTilez.Utilities;
using BuyTilez.Utilities.BrainTree;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BuyTilez.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly IProductRepository _productRepo;
        private readonly IApplicationUserRepository _userRepo;
        private readonly ICartRepository _cartRepo;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IEmailSender _emailSender;
        private readonly IOrderRepository _orderRepo;
        private readonly IOrderDetailsRepository _orderDetailRepo;
        private readonly ISaleRepository _saleRepo;
        private readonly ISaleDetailsRepository _saleDetailRepo;
        private readonly IBrainTreeGate _brain;

        [BindProperty]
        public ProductUserViewModel productUserViewModel { get; set; }

        public CartController(IWebHostEnvironment webHostEnvironment, IEmailSender emailSender, IProductRepository productRepo, IApplicationUserRepository userRepo,
                              IOrderRepository orderRepo, IOrderDetailsRepository orderDetailRepo, ISaleRepository saleRepo, ISaleDetailsRepository saleDetailRepo,
                              IBrainTreeGate brain, ICartRepository cartRepo)
        {
            _productRepo = productRepo;
            _userRepo = userRepo;
            _orderRepo = orderRepo;
            _orderDetailRepo = orderDetailRepo;
            _webHostEnvironment = webHostEnvironment;
            _emailSender = emailSender;
            _saleRepo = saleRepo;
            _saleDetailRepo = saleDetailRepo;
            _brain = brain;
            _cartRepo = cartRepo;
        }

        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var userId = claim.Value;

            List<ShoppingCart> cartList = _cartRepo.GetUserCart(userId).ToList();
            var productIdsInCart = cartList.Select(i => i.ProductId).ToList();
            var productList = _productRepo.GetAll(p => productIdsInCart.Contains(p.Id));

            var finalProductList = new List<Product>();

            foreach (var cartItem in cartList)
            {
                var tempProduct = productList.FirstOrDefault(p => p.Id == cartItem.ProductId);
                tempProduct.TempSquareMeters = cartItem.SquareMeters;
                finalProductList.Add(tempProduct);
            }

            return View(finalProductList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Index")]
        public IActionResult IndexPost(IEnumerable<Product> productList)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var userId = claim.Value;

            var cartList = new List<ShoppingCart>();
            foreach (var product in productList)
            {
                cartList.Add(new ShoppingCart { ProductId = product.Id, SquareMeters = product.TempSquareMeters, UserId = userId });
            }

            _cartRepo.ClearUserCart(userId);
            _cartRepo.AddRange(cartList);
            _cartRepo.Save();

            return RedirectToAction(nameof(Summary));
        }

        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var userId = claim.Value;

            ApplicationUser applicationUser;

            if (User.IsInRole(Constants.AdminRole))
            {
                if (HttpContext.Session.Get<int>(Constants.OrderSessionId) != 0)
                {
                    var order = _orderRepo.GetFirstOrDefault(u => u.Id == HttpContext.Session.Get<int>(Constants.OrderSessionId));
                    applicationUser = new ApplicationUser()
                    {
                        FullName = order.FullName,
                        Email = order.Email,
                        PhoneNumber = order.Phone
                    };
                }
                else
                {
                    applicationUser = new ApplicationUser();
                }
                var gateway = _brain.GetGateway();
                var clientToken = gateway.ClientToken.Generate();
                ViewBag.ClientToken = clientToken;
            }
            else
            {
                applicationUser = _userRepo.GetFirstOrDefault(u => u.Id == userId);
            }

            var cartList = _cartRepo.GetUserCart(userId).ToList();
            var productIdsInCart = cartList.Select(i => i.ProductId).ToList();
            var productList = _productRepo.GetAll(p => productIdsInCart.Contains(p.Id));

            productUserViewModel = new ProductUserViewModel()
            {
                ApplicationUser = applicationUser,
            };

            foreach (var cartItem in cartList)
            {
                var tempProduct = _productRepo.GetFirstOrDefault(p => p.Id == cartItem.ProductId);
                tempProduct.TempSquareMeters = cartItem.SquareMeters;
                productUserViewModel.ProductList.Add(tempProduct);
            }

            return View(productUserViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Summary")]
        public async Task<IActionResult> SummaryPost(IFormCollection collection, ProductUserViewModel productUserViewModel)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var userId = claim.Value;

            if (User.IsInRole(Constants.AdminRole))
            {
                var sale = new Sale()
                {
                    CreatedByUserId = userId,
                    TotalSaleAmount = productUserViewModel.ProductList.Sum(x => x.TempSquareMeters * x.Price),
                    Address = productUserViewModel.ApplicationUser.Address,
                    City = productUserViewModel.ApplicationUser.City,
                    Phone = productUserViewModel.ApplicationUser.PhoneNumber,
                    FullName = productUserViewModel.ApplicationUser.FullName,
                    Email = productUserViewModel.ApplicationUser.Email,
                    SaleDate = DateTime.Now,
                    SaleStatus = Constants.StatusPending,
                };

                _saleRepo.Add(sale);
                _saleRepo.Save();

                foreach (var product in productUserViewModel.ProductList)
                {
                    var saleDetail = new SaleDetails()
                    {
                        SaleId = sale.Id,
                        PricePerSquareMeter = product.Price,
                        SquareMeters = product.TempSquareMeters,
                        ProductId = product.Id
                    };
                    _saleDetailRepo.Add(saleDetail);
                }
                _saleDetailRepo.Save();

                var nonceFromClient = collection["payment_method_nonce"];

                var request = new TransactionRequest
                {
                    Amount = Convert.ToDecimal(sale.TotalSaleAmount),
                    PaymentMethodNonce = nonceFromClient,
                    OrderId = sale.Id.ToString(),
                    Options = new TransactionOptionsRequest
                    {
                        SubmitForSettlement = true
                    }
                };

                var gateway = _brain.GetGateway();
                var result = gateway.Transaction.Sale(request);

                if (result.Target.ProcessorResponseText == "Approved")
                {
                    sale.TransactionId = result.Target.Id;
                    sale.SaleStatus = Constants.StatusApproved;
                }
                else
                {
                    sale.SaleStatus = Constants.StatusCanceled;
                }

                _saleRepo.Save();
                return RedirectToAction(nameof(Confirmation), new { id = sale.Id });
            }

            return RedirectToAction(nameof(Confirmation));
        }

        public IActionResult Confirmation(int id = 0)
        {
            var sale = _saleRepo.GetFirstOrDefault(v => v.Id == id);
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var userId = claim.Value;

            _cartRepo.ClearUserCart(userId);
            HttpContext.Session.Clear();
            return View(sale);
        }

        public IActionResult Remove(int Id)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var userId = claim.Value;
            
            var cartList = _cartRepo.GetUserCart(userId).ToList();
            var cartItem = cartList.FirstOrDefault(p => p.ProductId == Id);
            if (cartItem != null)
            {
                _cartRepo.Remove(cartItem);
                _cartRepo.Save();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateCart(IEnumerable<Product> productList)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var userId = claim.Value;

            var cartList = productList.Select(prod => new ShoppingCart { ProductId = prod.Id, SquareMeters = prod.TempSquareMeters, UserId = userId }).ToList();

            _cartRepo.ClearUserCart(userId);
            _cartRepo.AddRange(cartList);
            _cartRepo.Save();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Clear()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var userId = claim.Value;

            _cartRepo.ClearUserCart(userId);
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }

}
