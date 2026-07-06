using GymManagementSystem_BLL.ViewModels.CategoryViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem_BLL.Interfaces
{
    public interface ICategoryService
    {
        Task<List<CategoryViewModel>> GetAllAsync();
        Task<CategoryViewModel?> GetByIdAsync(int id);
        Task<EditCategoryViewModel?> GetForEditAsync(int id);
        Task<bool> CreateAsync(CreateCategoryViewModel model);
        Task<bool> EditAsync(int id, EditCategoryViewModel model);
        Task<bool> DeleteAsync(int id);
    }
}
