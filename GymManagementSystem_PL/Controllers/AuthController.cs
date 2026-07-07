using GymManagementSystem_BLL.Interfaces;
using GymManagementSystem_BLL.ViewModels.AccountViewModels;
using GymManagementSystem_DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GymManagementSystem_PL.Controllers
{
    public class AuthController(IAccountService accountService, SignInManager<ApplicationUser> signInManager) : Controller
    {

        [HttpGet]
        public IActionResult Login()
        {
            // If user is already logged in, redirect directly to Home
            if (signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)   // Check if model state is valid , no errors
            {
                return View(model);
            }

            // SignInManager handles user lookup + password check + cookie issuance in one call
            var result = await signInManager.PasswordSignInAsync(
                model.Email,
                model.Password,
                model.RememberMe,
                lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }

            if (result.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "Your account is locked. Please try again later.");
            }
            else if (result.IsNotAllowed)
            {
                ModelState.AddModelError(string.Empty, "Your account is not allowed to sign in.");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid email or password.");
            }

            return View(model);
        }


        [HttpGet]
        [Authorize(Roles = "SuperAdmin")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var success = await accountService.RegisterAsync(model);

            if (!success)
            {
                ModelState.AddModelError(string.Empty, "Registration failed. Email may already exists");
                return View(model);
            }

            // After successful registration, redirect to Login
            return RedirectToAction(nameof(Login));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        [Authorize]
        public IActionResult LogoutConfirm()
        {
            return View();
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View(); // Return the AccessDenied View
        }
    }
}