using GymManagementSystem_BLL.Interfaces;
using GymManagementSystem_BLL.ViewModels.AnalyticsViewModels;
using GymManagementSystem_DAL.Data.DBContexts;
using Microsoft.EntityFrameworkCore;

namespace GymManagementSystem_BLL.Services
{
    public class AnalyticsService(GymDBContext context) : IAnalyticsService
    {
        public async Task<AnalyticsViewModel> GetAnalyticsDataAsync()
        {
            var now = DateTime.Now;

            return new AnalyticsViewModel
            {
                TotalMembers = await context.Members.CountAsync(),
                ActiveMembers = await context.MemberShips
                                        .CountAsync(ms => ms.EndDate >= now),
                TotalTrainers = await context.Trainers.CountAsync(),
                UpcomingSessions = await context.Sessions
                                        .CountAsync(s => s.StartDate > now),
                OngoingSessions = await context.Sessions
                                        .CountAsync(s => s.StartDate <= now && s.EndDate >= now),
                CompletedSessions = await context.Sessions
                                        .CountAsync(s => s.EndDate < now)
            };
        }
    }
}