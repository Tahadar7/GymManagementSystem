using GymManagementSystem_BLL.Interfaces;
using GymManagementSystem_BLL.ViewModels.SelectViewModels;
using GymManagementSystem_BLL.ViewModels.SessionViewModels;
using GymManagementSystem_DAL.Data.DBContexts;
using GymManagementSystem_DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GymManagementSystem_BLL.Services
{
    public class SessionService(GymDBContext context, ILogger<SessionService> logger) : ISessionService
    {
        public async Task<List<SessionViewModel>> GetAllAsync()
        {
            // Fetch all sessions with their realted Trainer and Category
            var sessions = await context.Sessions
                .AsNoTracking()
                .Include(s => s.SessionTrainer)
                .Include(s => s.SessionCategory)
                .ToListAsync();

            var sessionIds = sessions.Select(s => s.Id).ToList();  // Get all session Ids

            // Fetch booked counts for all sessions 
            var bookedCounts = await context.MemberSessions
                .Where(ms => sessionIds.Contains(ms.SessionId))
                .GroupBy(ms => ms.SessionId)
                .Select(g => new { SessionId = g.Key, Count = g.Count() })  // Group by SessionId and count the number of bookings
                .ToDictionaryAsync(x => x.SessionId, x => x.Count);

            // default to 0 if a session has no bookings
            return sessions
                .Select(s => MapToViewModel(s, bookedCounts.GetValueOrDefault(s.Id)))
                .ToList();
        }

        public async Task<SessionViewModel?> GetByIdAsync(int id)
        {
            var session = await context.Sessions
                .AsNoTracking()
                .Include(s => s.SessionTrainer)
                .Include(s => s.SessionCategory)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (session is null)
            {
                return null;
            }

            // Count the number of booked members for this session
            var bookedCount = await context.MemberSessions.CountAsync(ms => ms.SessionId == id);
            return MapToViewModel(session, bookedCount);
        }

        public async Task<EditSessionViewModel?> GetForEditAsync(int id)
        {
            var session = await context.Sessions.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);
            if (session is null || !await CanModifyAsync(session))
            {
                return null;
            }

            return new EditSessionViewModel
            {
                Description = session.Description,
                Capacity = session.Capacity,
                StartDate = session.StartDate,
                EndDate = session.EndDate,
                TrainerId = session.TrainerId,
                CategoryId = session.CategoryId
            };
        }

        public async Task<bool> CreateAsync(CreateSessionViewModel model)
        {
            try
            {
                logger.LogInformation("CreateSession called - TrainerId: {T}, CategoryId: {C}, Start: {S}, End: {E}",
            model.TrainerId, model.CategoryId, model.StartDate, model.EndDate);
                if (!await TrainerExistsAsync(model.TrainerId))
                {
                    return false;
                }

                if (!await CategoryExistsAsync(model.CategoryId))
                {
                    return false;
                }

                if (!IsValidDateRange(model.StartDate, model.EndDate))
                {
                    logger.LogWarning("Invalid date range - Start: {S}, End: {E}, Now: {N}",
                model.StartDate, model.EndDate, DateTime.Now);
                    return false;
                }

                var session = new Session
                {
                    Description = model.Description,
                    Capacity = model.Capacity,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    TrainerId = model.TrainerId,
                    CategoryId = model.CategoryId
                };

                context.Sessions.Add(session);
                return await context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to create session for TrainerId {TrainerId}", model.TrainerId);
                return false;
            }
        }

        public async Task<bool> EditAsync(int id, EditSessionViewModel model)
        {
            try
            {
                var session = await context.Sessions.FirstOrDefaultAsync(s => s.Id == id);
                if (session is null || !await CanModifyAsync(session))
                {
                    return false;
                }

                if (!await TrainerExistsAsync(model.TrainerId))
                {
                    return false;
                }

                if (!await CategoryExistsAsync(model.CategoryId))
                {
                    return false;
                }

                if (!IsValidDateRange(model.StartDate, model.EndDate))
                {
                    return false;
                }

                session.Description = model.Description;
                session.Capacity = model.Capacity;
                session.StartDate = model.StartDate;
                session.EndDate = model.EndDate;
                session.TrainerId = model.TrainerId;
                session.CategoryId = model.CategoryId;
                session.UpdatedAt = DateTime.Now;

                return await context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to edit session with Id {SessionId}", id);
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var session = await context.Sessions.FirstOrDefaultAsync(s => s.Id == id);
                if (session is null || !await CanModifyAsync(session))
                {
                    return false;
                }

                context.Sessions.Remove(session);
                return await context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to delete session with Id {SessionId}", id);
                return false;
            }
        }

        public async Task<List<TrainerSelectViewModel>> GetAllTrainersForDropdownAsync()
        {

            return await context.Trainers
                .AsNoTracking()
                .Select(t => new TrainerSelectViewModel { Id = t.Id, Name = t.Name })
                .ToListAsync();
        }

        public async Task<List<CategorySelectViewModel>> GetAllCategoriesForDropdownAsync()
        {
            return await context.Categories
                .AsNoTracking()
                .Select(c => new CategorySelectViewModel { Id = c.Id, Name = c.CategoryName })
                .ToListAsync();
        }

        #region Helper Methods
        private static SessionViewModel MapToViewModel(Session session, int bookedCount)
        {
            return new SessionViewModel
            {
                Id = session.Id,
                Description = session.Description,
                CategoryName = session.SessionCategory.CategoryName,
                TrainerName = session.SessionTrainer.Name,
                StartDate = session.StartDate,
                EndDate = session.EndDate,
                Capacity = session.Capacity,
                AvailableSlots = session.Capacity - bookedCount
            };
        }

        // A session can be edited/deleted only if not started yet or has no bookings
        private async Task<bool> CanModifyAsync(Session session)
        {
            if (session.StartDate <= DateTime.Now)
            {
                return false;
            }

            var hasBookings = await context.MemberSessions.AnyAsync(ms => ms.SessionId == session.Id);
            if (hasBookings)
            {
                return false;
            }

            return true;
        }

        private async Task<bool> TrainerExistsAsync(int trainerId)
        {
            return await context.Trainers.AnyAsync(t => t.Id == trainerId);
        }

        private async Task<bool> CategoryExistsAsync(int categoryId)
        {
            return await context.Categories.AnyAsync(c => c.Id == categoryId);
        }

        // Allow sessions starting within the next 5 minutes to account for form fill time
        private static bool IsValidDateRange(DateTime start, DateTime end) =>
            start < end && start >= DateTime.Now.AddMinutes(-5);

        #endregion
    }
}