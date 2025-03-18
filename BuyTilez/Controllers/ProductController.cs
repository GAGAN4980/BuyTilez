using BuyTilez.Data.Data.Repository.IRepository;
using BuyTilez.Models;
using BuyTilez.Models.ViewModels;
using BuyTilez.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BuyTilez.Controllers
{
    [Authorize(Roles = Constants.AdminRole)]
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepo;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(IProductRepository productRepo, IWebHostEnvironment webHostEnvironment)
        {
            _productRepo = productRepo;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            // Repository Pattern is used
            IEnumerable<Product> list = _productRepo.GetAll(includeProperties: "Category,ApplicationType");
            return View(list);
        }

        // GET
        public IActionResult Upsert(int? Id)
        {
            ProductViewModel productViewModel = new ProductViewModel()
            {
                Product = new Product(),
                CategoryList = _productRepo.GetAllDropdownList(Constants.CategoryName),
                ApplicationTypeList = _productRepo.GetAllDropdownList(Constants.ApplicationTypeName)
            };

            if (Id == null)
            {
                // Create a new product
                return View(productViewModel);
            }
            else
            {
                productViewModel.Product = _productRepo.Get(Id.GetValueOrDefault());
                if (productViewModel == null)
                {
                    return NotFound();
                }
                return View(productViewModel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductViewModel productViewModel)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                string webRootPath = _webHostEnvironment.WebRootPath;

                if (productViewModel.Product.Id == 0)
                {
                    // Create
                    string upload = webRootPath + Constants.ImagePath;
                    string fileName = Guid.NewGuid().ToString();
                    string extension = Path.GetExtension(files[0].FileName);

                    using (var filesStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                    {
                        files[0].CopyTo(filesStream);
                    }

                    productViewModel.Product.ImageUrl = fileName + extension;
                    _productRepo.Add(productViewModel.Product);
                }
                else
                {
                    // Update
                    var existingProduct = _productRepo.GetFirstOrDefault(p => p.Id == productViewModel.Product.Id, isTracking: false);

                    if (files.Count > 0) // If a new image is uploaded
                    {
                        string upload = webRootPath + Constants.ImagePath;
                        string fileName = Guid.NewGuid().ToString();
                        string extension = Path.GetExtension(files[0].FileName);

                        // Delete old image
                        var oldFile = Path.Combine(upload, existingProduct.ImageUrl);
                        if (System.IO.File.Exists(oldFile))
                        {
                            System.IO.File.Delete(oldFile);
                        }

                        // Upload new image
                        using (var filesStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                        {
                            files[0].CopyTo(filesStream);
                        }

                        productViewModel.Product.ImageUrl = fileName + extension;
                    }
                    else
                    {
                        productViewModel.Product.ImageUrl = existingProduct.ImageUrl;
                    }
                    _productRepo.Update(productViewModel.Product);
                }

                _productRepo.Save();
                return RedirectToAction("Index");
            }

            // Reload dropdown lists in case of failure
            productViewModel.CategoryList = _productRepo.GetAllDropdownList(Constants.CategoryName);
            productViewModel.ApplicationTypeList = _productRepo.GetAllDropdownList(Constants.ApplicationTypeName);

            return View(productViewModel);
        }

        // GET: Delete
        public IActionResult Delete(int? Id)
        {
            if (Id == null || Id == 0)
            {
                return NotFound();
            }

            Product product = _productRepo.GetFirstOrDefault(p => p.Id == Id, includeProperties: "Category,ApplicationType");

            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(Product product)
        {
            if (product == null)
            {
                return NotFound();
            }

            // Delete image
            string upload = _webHostEnvironment.WebRootPath + Constants.ImagePath;

            var oldFile = Path.Combine(upload, product.ImageUrl);
            if (System.IO.File.Exists(oldFile))
            {
                System.IO.File.Delete(oldFile);
            }

            _productRepo.Remove(product);
            _productRepo.Save();
            return RedirectToAction(nameof(Index));
        }
    }
}
