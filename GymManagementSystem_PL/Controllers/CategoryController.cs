using GymManagementSystem_BLL.Interfaces;
using GymManagementSystem_BLL.ViewModels.CategoryViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymManagementSystem_PL.Controllers
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class CategoryController(ICategoryService categoryService) : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var categories = await categoryService.GetAllAsync();
            return View(categories);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCategoryViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var success = await categoryService.CreateAsync(model);
            if (success)
                TempData["SuccessMessage"] = "Category created successfully.";
            else
                TempData["ErrorMessage"] = "Failed to create category. Name may already exist.";

            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var category = await categoryService.GetForEditAsync(id);
            if (category is null)
            {
                TempData["ErrorMessage"] = "Category not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditCategoryViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var success = await categoryService.EditAsync(id, model);
            if (success)
                TempData["SuccessMessage"] = "Category updated successfully.";
            else
                TempData["ErrorMessage"] = "Failed to update category. Name may already be taken.";

            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await categoryService.GetByIdAsync(id);
            if (category is null)
            {
                TempData["ErrorMessage"] = "Category not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var success = await categoryService.DeleteAsync(id);
            if (success)
                TempData["SuccessMessage"] = "Category deleted successfully.";
            else
                TempData["ErrorMessage"] = "Failed to delete category. It may have sessions assigned to it.";

            return RedirectToAction(nameof(Index));
        }
    }
}