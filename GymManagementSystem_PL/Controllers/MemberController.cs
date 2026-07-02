using GymManagementSystem_BLL.Interfaces;
using GymManagementSystem_BLL.ViewModels.MemberViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymManagementSystem_PL.Controllers
{
    [Authorize]
    public class MemberController(IMemberService memberService) : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var members = await memberService.GetAllAsync();
            return View(members);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var member = await memberService.GetByIdAsync(id);
            if (member is null)
            {
                TempData["ErrorMessage"] = "Member not found";
                return RedirectToAction(nameof(Index));
            }
            return View(member);
        }

        [HttpGet]
        public async Task<IActionResult> HealthRecord(int id)
        {
            var healthRecord = await memberService.GetHealthRecordAsync(id);
            if (healthRecord is null)
            {
                TempData["ErrorMessage"] = "Health record not found";
                return RedirectToAction(nameof(Index));
            }
            return View(healthRecord);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();    // Return the view for creating a new member
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateMemberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var success = await memberService.CreateAsync(model);
            if (success)
            {
                TempData["SuccessMessage"] = "Member created successfully";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to create member";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var member = await memberService.GetForEditAsync(id);
            if (member is null)
            {
                TempData["ErrorMessage"] = "Member not found";
                return RedirectToAction(nameof(Index));
            }
            return View(member);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditMemberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var success = await memberService.EditAsync(id, model);
            if (success)
            {
                TempData["SuccessMessage"] = "Member updated successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to update member.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var member = await memberService.GetByIdAsync(id);
            if (member is null)
            {
                TempData["ErrorMessage"] = "Member not found";
                return RedirectToAction(nameof(Index));
            }

            // Pass the member to the view so confirmation can be displayed
            return View(member);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")] // keeps the POST route as /Member/Delete/{id}
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var success = await memberService.DeleteAsync(id);
            if (success)
            {
                TempData["SuccessMessage"] = "Member deleted successfully";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to delete member";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}