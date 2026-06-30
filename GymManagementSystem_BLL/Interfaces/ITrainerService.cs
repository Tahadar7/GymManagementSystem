using GymManagementSystem_BLL.ViewModels.TrainerViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem_BLL.Interfaces
{
    public interface ITrainerService
    {
        Task<List<TrainerViewModel>> GetAllAsync();
        Task<TrainerViewModel?> GetByIdAsync(int id);
        Task<EditTrainerViewModel?> GetForEditAsync(int id);
        Task<bool> CreateAsync(CreateTrainerViewModel model);
        Task<bool> EditAsync(int id, EditTrainerViewModel model);
        Task<bool> DeleteAsync(int id);
    }
}
