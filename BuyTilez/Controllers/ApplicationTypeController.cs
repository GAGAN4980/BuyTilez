using BuyTilez.Data.Data.Repository.IRepository;
using BuyTilez.Models;
using BuyTilez.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BuyTilez.Controllers
{
    [Authorize(Roles = Constants.AdminRole)]
    public class ApplicationTypeController : Controller
    {
        private readonly IApplicationTypeRepository _applicationTypeRepo;

        public ApplicationTypeController(IApplicationTypeRepository applicationTypeRepo)
        {
            _applicationTypeRepo = applicationTypeRepo;
        }

        public IActionResult Index()
        {
            IEnumerable<ApplicationType> list = _applicationTypeRepo.GetAll();
            return View(list);
        }

        // GET: Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ApplicationType applicationType)
        {
            if (ModelState.IsValid)
            {
                _applicationTypeRepo.Add(applicationType);
                _applicationTypeRepo.Save();
                return RedirectToAction(nameof(Index));
            }
            return View(applicationType);
        }

        // GET: Edit
        public IActionResult Edit(int? Id)
        {
            if (Id == null || Id == 0)
            {
                return NotFound();
            }
            var obj = _applicationTypeRepo.Get(Id.GetValueOrDefault());
            if (obj == null)
            {
                return NotFound();
            }
            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ApplicationType applicationType)
        {
            if (ModelState.IsValid)
            {
                _applicationTypeRepo.Update(applicationType);
                _applicationTypeRepo.Save();
                return RedirectToAction(nameof(Index));
            }
            return View(applicationType);
        }

        // GET: Delete
        public IActionResult Delete(int? Id)
        {
            if (Id == null || Id == 0)
            {
                return NotFound();
            }
            var obj = _applicationTypeRepo.Get(Id.GetValueOrDefault());
            if (obj == null)
            {
                return NotFound();
            }
            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(ApplicationType applicationType)
        {
            if (applicationType == null)
            {
                return NotFound();
            }
            _applicationTypeRepo.Remove(applicationType);
            _applicationTypeRepo.Save();
            return RedirectToAction(nameof(Index));
        }
    }
}
