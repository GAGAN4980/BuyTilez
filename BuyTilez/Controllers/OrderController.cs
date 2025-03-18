using BuyTilez.Data.Data.Repository.IRepository;
using BuyTilez.Models.ViewModels;
using BuyTilez.Models;
using BuyTilez.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BuyTilez.Controllers
{
    [Authorize(Roles = Constants.AdminRole)]
    public class OrderController : Controller
    {
        private readonly IOrderRepository _orderRepo;
        private readonly IOrderDetailsRepository _orderDetailRepo;

        [BindProperty]
        public OrderViewModel OrderViewModel { get; set; }

        public OrderController(IOrderRepository orderRepo, IOrderDetailsRepository orderDetailRepo)
        {
            _orderRepo = orderRepo;
            _orderDetailRepo = orderDetailRepo;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(int id)
        {
            OrderViewModel = new OrderViewModel()
            {
                Order = _orderRepo.GetFirstOrDefault(o => o.Id == id),
                OrderDetails = _orderDetailRepo.GetAll(u => u.OrderId == id, includeProperties: "Product")
            };

            return View(OrderViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Details()
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            OrderViewModel.OrderDetails = _orderDetailRepo.GetAll(d => d.OrderId == OrderViewModel.Order.Id);
            foreach (var detail in OrderViewModel.OrderDetails)
            {
                ShoppingCart shoppingCart = new ShoppingCart()
                {
                    ProductId = detail.ProductId
                };
                shoppingCartList.Add(shoppingCart);
            }
            HttpContext.Session.Clear();
            HttpContext.Session.Set(Constants.ShoppingCartSession, shoppingCartList);
            HttpContext.Session.Set(Constants.OrderSessionId, OrderViewModel.Order.Id);
            return RedirectToAction("Index", "Cart");
        }

        [HttpPost]
        public IActionResult Delete()
        {
            Order order = _orderRepo.GetFirstOrDefault(o => o.Id == OrderViewModel.Order.Id);
            IEnumerable<OrderDetails> orderDetails = _orderDetailRepo.GetAll(o => o.OrderId == OrderViewModel.Order.Id);

            _orderDetailRepo.RemoveRange(orderDetails);
            _orderRepo.Remove(order);
            _orderRepo.Save();

            return RedirectToAction(nameof(Index));
        }

        #region APIs

        [HttpGet]
        public IActionResult GetOrderList()
        {
            return Json(new { data = _orderRepo.GetAll() });
        }

        #endregion
    }
}