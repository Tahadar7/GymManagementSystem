using System;
using System.Collections.Generic;
using System.Text;
using GymManagementSystem_BLL.ViewModels.MemberViewModels;
using GymManagementSystem_BLL.ViewModels.HealthRecordViewModels;

namespace GymManagementSystem_BLL.Interfaces
{
    public interface IMemberService
    {
        Task<List<MemberViewModel>> GetAllAsync();
        Task<MemberViewModel?> GetByIdAsync(int id);
        Task<HealthRecordViewModel?> GetHealthRecordAsync(int memberId);
        Task<EditMemberViewModel?> GetForEditAsync(int id);
        Task<bool> CreateAsync(CreateMemberViewModel model);
        Task<bool> EditAsync(int id, EditMemberViewModel model);
        Task<bool> DeleteAsync(int id);
    }
}
