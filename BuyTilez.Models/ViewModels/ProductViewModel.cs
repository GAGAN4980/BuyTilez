using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuyTilez.Models.ViewModels
{
    public class ProductViewModel
    {
        public Product Product { get; set; }

        public IEnumerable<SelectListItem>? CategoryList { get; set; }

        public IEnumerable<SelectListItem>? ApplicationTypeList { get; set; }
    }
}
