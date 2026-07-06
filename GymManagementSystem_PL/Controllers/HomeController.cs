using GymManagementSystem_BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymManagementSystem_PL.Controllers
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class HomeController(IAnalyticsService analyticsService) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var data = await analyticsService.GetAnalyticsDataAsync();
            return View(data);
        }
    }
}