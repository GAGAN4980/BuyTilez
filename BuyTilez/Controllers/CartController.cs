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
                              IBrainTreeGate brain)
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
        }

        public IActionResult Index()
        {
            List<ShoppingCart> cartList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(Constants.ShoppingCartSession) != null &&
                HttpContext.Session.Get<IEnumerable<ShoppingCart>>(Constants.ShoppingCartSession).Count() > 0)
            {
                cartList = HttpContext.Session.Get<List<ShoppingCart>>(Constants.ShoppingCartSession);
            }
            List<int> productIdsInCart = cartList.Select(i => i.ProductId).ToList();
            IEnumerable<Product> productList = _productRepo.GetAll(p => productIdsInCart.Contains(p.Id));

            List<Product> finalProductList = new List<Product>();

            foreach (var cartItem in cartList)
            {
                Product tempProduct = productList.FirstOrDefault(p => p.Id == cartItem.ProductId);
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
            List<ShoppingCart> cartList = new List<ShoppingCart>();
            foreach (Product product in productList)
            {
                cartList.Add(new ShoppingCart { ProductId = product.Id, SquareMeters = product.TempSquareMeters });
            }
            HttpContext.Session.Set(Constants.ShoppingCartSession, cartList);
            return RedirectToAction(nameof(Summary));
        }

        public IActionResult Summary()
        {
            ApplicationUser applicationUser;

            if (User.IsInRole(Constants.AdminRole))
            {
                if (HttpContext.Session.Get<int>(Constants.OrderSessionId) != 0)
                {
                    Order order = _orderRepo.GetFirstOrDefault(u => u.Id == HttpContext.Session.Get<int>(Constants.OrderSessionId));
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
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                applicationUser = _userRepo.GetFirstOrDefault(u => u.Id == claim.Value);
            }

            List<ShoppingCart> cartList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(Constants.ShoppingCartSession) != null &&
                HttpContext.Session.Get<IEnumerable<ShoppingCart>>(Constants.ShoppingCartSession).Count() > 0)
            {
                cartList = HttpContext.Session.Get<List<ShoppingCart>>(Constants.ShoppingCartSession);
            }
            List<int> productIdsInCart = cartList.Select(i => i.ProductId).ToList();
            IEnumerable<Product> productList = _productRepo.GetAll(p => productIdsInCart.Contains(p.Id));

            productUserViewModel = new ProductUserViewModel()
            {
                ApplicationUser = applicationUser,
            };

            foreach (var cartItem in cartList)
            {
                Product tempProduct = _productRepo.GetFirstOrDefault(p => p.Id == cartItem.ProductId);
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

            if (User.IsInRole(Constants.AdminRole))
            {
                Sale sale = new Sale()
                {
                    CreatedByUserId = claim.Value,
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
                    SaleDetails saleDetail = new SaleDetails()
                    {
                        SaleId = sale.Id,
                        PricePerSquareMeter = product.Price,
                        SquareMeters = product.TempSquareMeters,
                        ProductId = product.Id
                    };
                    _saleDetailRepo.Add(saleDetail);
                }
                _saleDetailRepo.Save();

                string nonceFromClient = collection["payment_method_nonce"];

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
                Result<Transaction> result = gateway.Transaction.Sale(request);

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
            Sale sale = _saleRepo.GetFirstOrDefault(v => v.Id == id);
            HttpContext.Session.Clear();
            return View(sale);
        }

        public IActionResult Remove(int Id)
        {
            List<ShoppingCart> cartList = HttpContext.Session.Get<List<ShoppingCart>>(Constants.ShoppingCartSession);
            cartList.Remove(cartList.FirstOrDefault(p => p.ProductId == Id));
            HttpContext.Session.Set(Constants.ShoppingCartSession, cartList);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateCart(IEnumerable<Product> productList)
        {
            List<ShoppingCart> cartList = productList.Select(prod => new ShoppingCart { ProductId = prod.Id, SquareMeters = prod.TempSquareMeters }).ToList();
            HttpContext.Session.Set(Constants.ShoppingCartSession, cartList);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Clear()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
