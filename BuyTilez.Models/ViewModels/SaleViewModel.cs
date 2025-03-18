using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuyTilez.Models.ViewModels
{
    public class SaleViewModel
    {
        public IEnumerable<Sale> SaleList { get; set; }

        public IEnumerable<SelectListItem> StatusList { get; set; }

        public string Status { get; set; }
    }
}
