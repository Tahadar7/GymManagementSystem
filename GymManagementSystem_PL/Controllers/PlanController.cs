using GymManagementSystem_BLL.Interfaces;
using GymManagementSystem_BLL.ViewModels.PlanViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymManagementSystem_PL.Controllers
{
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class PlanController(IPlanService planService) : Controller
    {

        public async Task<IActionResult> Index()
        {
            var plans = await planService.GetAllAsync();
            return View(plans);
        }

        public async Task<IActionResult> Details(int id)
        {
            var plan = await planService.GetByIdAsync(id);
            if (plan is null)
            {
                TempData["ErrorMessage"] = "Plan not found";
                return RedirectToAction(nameof(Index));
            }
            return View(plan);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreatePlanViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var success = await planService.CreateAsync(model);
            if (success)
            {
                TempData["SuccessMessage"] = "Plan created successfully";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to create plan";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var plan = await planService.GetForEditAsync(id);
            if (plan is null)
            {
                TempData["ErrorMessage"] = "Plan not found or cannot be edited";
                return RedirectToAction(nameof(Index));
            }
            return View(plan);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditPlanViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var success = await planService.EditAsync(id, model);
            if (success)
            {
                TempData["SuccessMessage"] = "Plan updated successfully";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to update plan";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var success = await planService.ToggleStatusAsync(id);
            if (success)
            {
                TempData["SuccessMessage"] = "Plan status changed successfully";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to change plan status";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}