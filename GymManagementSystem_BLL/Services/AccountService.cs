using GymManagementSystem_BLL.Interfaces;
using GymManagementSystem_BLL.ViewModels.AccountViewModels;
using GymManagementSystem_DAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace GymManagementSystem_BLL.Services
{
    public class AccountService(UserManager<ApplicationUser> userManager, ILogger<AccountService> logger) : IAccountService
    {
        public async Task<bool> RegisterAsync(RegisterViewModel model)
        {
            try
            {
                var existingUser = await userManager.FindByEmailAsync(model.Email);
                if (existingUser is not null)
                {
                    return false;
                }

                var user = new ApplicationUser
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    UserName = model.Email // Identity uses UserName for login so set email to username
                };

                // CreateAsync handles password hashing automatically
                var result = await userManager.CreateAsync(user, model.Password);

                if (!result.Succeeded)  // Check if result is not successful
                {
                    // Log each Identity error
                    foreach (var error in result.Errors)
                    {
                        logger.LogError("Registration failed for {Email}: {Code} - {Description}",
                            model.Email, error.Code, error.Description);
                    }
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error during registration for {Email}", model.Email);
                return false;
            }
        }
    }
}