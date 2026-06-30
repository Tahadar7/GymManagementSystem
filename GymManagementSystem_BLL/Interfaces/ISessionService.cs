using GymManagementSystem_BLL.ViewModels.SelectViewModels;
using GymManagementSystem_BLL.ViewModels.SessionViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem_BLL.Interfaces
{
    public interface ISessionService
    {
        Task<List<SessionViewModel>> GetAllAsync();
        Task<SessionViewModel?> GetByIdAsync(int id);
        Task<EditSessionViewModel?> GetForEditAsync(int id);
        Task<bool> CreateAsync(CreateSessionViewModel model);
        Task<bool> EditAsync(int id, EditSessionViewModel model);
        Task<bool> DeleteAsync(int id);

        Task<List<TrainerSelectViewModel>> GetAllTrainersForDropdownAsync();
        Task<List<CategorySelectViewModel>> GetAllCategoriesForDropdownAsync();
    }
}
