using BuyTilez.Data.Data.Repository.IRepository;
using BuyTilez.Models.ViewModels;
using BuyTilez.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuyTilez.Controllers
{
    public class SaleController : Controller
    {
        private readonly ISaleRepository _saleRepo;
        private readonly ISaleDetailsRepository _saleDetailRepo;

        public SaleController(ISaleRepository saleRepo, ISaleDetailsRepository saleDetailRepo)
        {
            _saleRepo = saleRepo;
            _saleDetailRepo = saleDetailRepo;
        }

        public IActionResult Index(string searchName = null, string searchEmail = null, string searchPhone = null, string status = null)
        {
            SaleViewModel saleViewModel = new SaleViewModel()
            {
                SaleList = _saleRepo.GetAll(),
                StatusList = Constants.StatusList.ToList().Select(l => new SelectListItem
                {
                    Text = l,
                    Value = l
                })
            };

            if (!string.IsNullOrEmpty(searchName))
            {
                saleViewModel.SaleList = saleViewModel.SaleList.Where(u => u.FullName.ToLower().Contains(searchName.ToLower()));
            }

            if (!string.IsNullOrEmpty(searchEmail))
            {
                saleViewModel.SaleList = saleViewModel.SaleList.Where(u => u.Email.ToLower().Contains(searchEmail.ToLower()));
            }

            if (!string.IsNullOrEmpty(searchPhone))
            {
                saleViewModel.SaleList = saleViewModel.SaleList.Where(u => u.Phone.ToLower().Contains(searchPhone.ToLower()));
            }

            if (!string.IsNullOrEmpty(status) && status != "--Status--")
            {
                saleViewModel.SaleList = saleViewModel.SaleList.Where(u => u.SaleStatus.ToLower().Contains(status.ToLower()));
            }

            return View(saleViewModel);
        }
    }
}
