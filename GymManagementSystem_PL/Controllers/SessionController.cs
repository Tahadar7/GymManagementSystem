using GymManagementSystem_BLL.Interfaces;
using GymManagementSystem_BLL.ViewModels.SessionViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GymManagementSystem_PL.Controllers
{
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class SessionController(ISessionService sessionService) : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var sessions = await sessionService.GetAllAsync();
            return View(sessions);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var session = await sessionService.GetByIdAsync(id);
            if (session is null)
            {
                TempData["ErrorMessage"] = "Session not found";
                return RedirectToAction(nameof(Index));
            }
            return View(session);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadDropdownsAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateSessionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdownsAsync();
                return View(model);
            }

            var success = await sessionService.CreateAsync(model);
            if (success)
            {
                TempData["SuccessMessage"] = "Session created successfully";
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = "Failed to create session";
            await LoadDropdownsAsync();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var session = await sessionService.GetForEditAsync(id);
            if (session is null)
            {
                TempData["ErrorMessage"] = "Session may be ongoing or has members";
                return RedirectToAction(nameof(Index));
            }

            await LoadDropdownsAsync();
            return View(session);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditSessionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdownsAsync();
                return View(model);
            }

            var success = await sessionService.EditAsync(id, model);
            if (success)
            {
                TempData["SuccessMessage"] = "Session updated successfully";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to update session. It may have already started or has bookings";
            }

            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var session = await sessionService.GetByIdAsync(id);
            if (session is null)
            {
                TempData["ErrorMessage"] = "Session not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(session);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var success = await sessionService.DeleteAsync(id);
            if (success)
            {
                TempData["SuccessMessage"] = "Session deleted successfully";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to delete session. It may have already started or has bookings.";
            }

            return RedirectToAction(nameof(Index));
        }

        // Loads both Trainer and Category dropdowns into ViewBag
        private async Task LoadDropdownsAsync()
        {
            var trainers = await sessionService.GetAllTrainersForDropdownAsync();
            var categories = await sessionService.GetAllCategoriesForDropdownAsync();

            ViewBag.Trainers = new SelectList(trainers, "Id", "Name");
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
        }

    }
}