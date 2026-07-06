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
                // Check if user already exists
                var existingUser = await userManager.FindByEmailAsync(model.Email);
                if (existingUser is not null)
                {
                    logger.LogWarning("Registration attempted with existing email: {Email}", model.Email);
                    return false;
                }

                // Create user object
                var user = new ApplicationUser
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    UserName = model.Email
                };

                // Create user in database
                var result = await userManager.CreateAsync(user, model.Password);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        logger.LogError("Registration failed for {Email}: {Code} - {Description}",
                            model.Email, error.Code, error.Description);
                    }
                    return false;
                }

                // Assign Admin role
                var roleResult = await userManager.AddToRoleAsync(user, "Admin");
                if (!roleResult.Succeeded)
                {
                    // Delete the user since role assignment failed
                    var deleteResult = await userManager.DeleteAsync(user);

                    if (deleteResult.Succeeded)
                    {
                        logger.LogError("Failed to assign Admin role to {Email}. User was deleted.",
                            model.Email);
                    }

                    return false;
                }

                // Everything succeeded
                logger.LogInformation("User {Email} registered successfully with Admin role. UserId: {UserId}",
                    model.Email, user.Id);
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