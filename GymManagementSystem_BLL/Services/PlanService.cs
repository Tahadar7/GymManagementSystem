using GymManagementSystem_BLL.Interfaces;
using GymManagementSystem_BLL.ViewModels.PlanViewModels;
using GymManagementSystem_DAL.Data.DBContexts;
using GymManagementSystem_DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GymManagementSystem_BLL.Services
{
    public class PlanService(GymDBContext context, ILogger<PlanService> logger) : IPlanService
    {
        public async Task<List<PlanViewModel>> GetAllAsync()
        {
            var plans = await context.Plans.AsNoTracking().ToListAsync();
            return plans.Select(MapToViewModel).ToList();
        }

        public async Task<PlanViewModel?> GetByIdAsync(int id)
        {
            var plan = await context.Plans.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
            if (plan is null)
            {
                return null;
            }

            return MapToViewModel(plan);
        }

        public async Task<EditPlanViewModel?> GetForEditAsync(int id)
        {
            var plan = await context.Plans.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
            if (plan is null)
            {
                return null;
            }

            // Block loading the edit form at all if members are actively subscribed
            if (await HasActiveMembershipsAsync(id))
            {
                return null;
            }

            return new EditPlanViewModel
            {
                Name = plan.Name,
                Description = plan.Description,
                DurationDays = plan.DurationDays,
                Price = plan.Price,
                IsActive = plan.IsActive
            };
        }

        public async Task<bool> CreateAsync(CreatePlanViewModel model)
        {
            try
            {
                var plan = new Plan
                {
                    Name = model.Name,
                    Description = model.Description,
                    DurationDays = model.DurationDays,
                    Price = model.Price,
                    IsActive = model.IsActive
                };

                context.Plans.Add(plan);
                return await context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to create plan with name {PlanName}", model.Name);
                return false;
            }
        }
        public async Task<bool> EditAsync(int id, EditPlanViewModel model)
        {
            try
            {
                var plan = await context.Plans.FirstOrDefaultAsync(p => p.Id == id);
                if (plan is null)
                {
                    return false;
                }

                // Blocks editing price/duration while members are actively subscribed
                if (await HasActiveMembershipsAsync(id))
                {
                    return false;
                }

                plan.Name = model.Name;
                plan.Description = model.Description;
                plan.DurationDays = model.DurationDays;
                plan.Price = model.Price;
                plan.IsActive = model.IsActive;
                plan.UpdatedAt = DateTime.Now;

                return await context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to edit plan with Id {PlanId}", id);
                return false;
            }
        }

        public async Task<bool> ToggleStatusAsync(int id)
        {
            try
            {
                var plan = await context.Plans.FirstOrDefaultAsync(p => p.Id == id);
                if (plan is null)
                {
                    return false;
                }

                if (await HasActiveMembershipsAsync(id))
                {
                    return false;
                }

                plan.IsActive = !plan.IsActive;
                plan.UpdatedAt = DateTime.Now;

                return await context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to toggle status for plan with Id {PlanId}", id);
                return false;
            }
        }

        #region Helper Methods
        private static PlanViewModel MapToViewModel(Plan plan)
        {
            return new PlanViewModel
            {
                Id = plan.Id,
                Name = plan.Name,
                Description = plan.Description,
                DurationDays = plan.DurationDays,
                Price = plan.Price,
                IsActive = plan.IsActive
            };
        }

        // Active membership means end date is not passed yet
        private async Task<bool> HasActiveMembershipsAsync(int planId)
        {
            return await context.MemberShips.AnyAsync(ms => ms.PlanId == planId && ms.EndDate >= DateTime.Now);
        }

        #endregion
    }
}