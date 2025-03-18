using BuyTilez.Data.Data.Repository.IRepository;
using BuyTilez.Models;
using BuyTilez.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BuyTilez.Controllers
{
    [Authorize(Roles = Constants.AdminRole)]
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _catRepo;

        public CategoryController(ICategoryRepository catRepo)
        {
            _catRepo = catRepo;
        }

        public IActionResult Index()
        {
            IEnumerable<Category> categoryList = _catRepo.GetAll();
            return View(categoryList);
        }

        // GET: Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                _catRepo.Add(category);
                _catRepo.Save();
                TempData[Constants.Success] = "Category created successfully";

                return RedirectToAction(nameof(Index));
            }
            TempData[Constants.Error] = "Error creating new category";
            return View(category);
        }

        // GET: Edit
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var obj = _catRepo.Get(id.GetValueOrDefault());
            if (obj == null)
            {
                return NotFound();
            }

            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                _catRepo.Update(category);
                _catRepo.Save();
                TempData[Constants.Success] = "Category updated successfully";

                return RedirectToAction(nameof(Index));
            }

            TempData[Constants.Error] = "Error updating category";
            return View(category);
        }

        // GET: Delete
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var obj = _catRepo.Get(id.GetValueOrDefault());
            if (obj == null)
            {
                return NotFound();
            }

            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(Category category)
        {
            if (category == null)
            {
                return NotFound();
            }

            _catRepo.Remove(category);
            _catRepo.Save();
            TempData[Constants.Success] = "Category deleted successfully";

            return RedirectToAction(nameof(Index));
        }
    }
}
