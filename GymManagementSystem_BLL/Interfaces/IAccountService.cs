using GymManagementSystem_BLL.ViewModels.AccountViewModels;

namespace GymManagementSystem_BLL.Interfaces
{
    public interface IAccountService
    {
        Task<bool> RegisterAsync(RegisterViewModel model);
    }
}