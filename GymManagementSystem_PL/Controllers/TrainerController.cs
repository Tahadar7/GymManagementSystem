using GymManagementSystem_BLL.Interfaces;
using GymManagementSystem_BLL.ViewModels.TrainerViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymManagementSystem_PL.Controllers
{
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class TrainerController(ITrainerService trainerService) : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var trainers = await trainerService.GetAllAsync();
            return View(trainers);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var trainer = await trainerService.GetByIdAsync(id);
            if (trainer is null)
            {
                TempData["ErrorMessage"] = "Trainer not found";
                return RedirectToAction(nameof(Index));
            }
            return View(trainer);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateTrainerViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var success = await trainerService.CreateAsync(model);
            if (success)
            {
                TempData["SuccessMessage"] = "Trainer created successfully";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to create trainer";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var trainer = await trainerService.GetForEditAsync(id);
            if (trainer is null)
            {
                TempData["ErrorMessage"] = "Trainer not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(trainer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditTrainerViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var success = await trainerService.EditAsync(id, model);
            if (success)
            {
                TempData["SuccessMessage"] = "Trainer updated successfully";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to update trainer";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var trainer = await trainerService.GetByIdAsync(id);
            if (trainer is null)
            {
                TempData["ErrorMessage"] = "Trainer not found";
                return RedirectToAction(nameof(Index));
            }
            return View(trainer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var success = await trainerService.DeleteAsync(id);
            if (success)
            {
                TempData["SuccessMessage"] = "Trainer deleted successfully";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to delete trainer";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}