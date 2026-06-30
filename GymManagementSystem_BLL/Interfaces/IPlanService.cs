using GymManagementSystem_BLL.ViewModels.PlanViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem_BLL.Interfaces
{
    public interface IPlanService
    {
        Task<List<PlanViewModel>> GetAllAsync();
        Task<PlanViewModel?> GetByIdAsync(int id);
        Task<EditPlanViewModel?> GetForEditAsync(int id);
        Task<bool> CreateAsync(CreatePlanViewModel model);
        Task<bool> EditAsync(int id, EditPlanViewModel model);
        Task<bool> ToggleStatusAsync(int id);
    }
}
