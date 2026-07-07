using GymManagementSystem_BLL.Interfaces;
using GymManagementSystem_BLL.ViewModels.HealthRecordViewModels;
using GymManagementSystem_BLL.ViewModels.MemberViewModels;
using GymManagementSystem_BLL.ViewModels.PlanViewModels;
using GymManagementSystem_BLL.ViewModels.SessionViewModels;
using GymManagementSystem_DAL.Data.DBContexts;
using GymManagementSystem_DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GymManagementSystem_BLL.Services
{
    public class MemberService(GymDBContext context, IAttachmentService attachment, ILogger<MemberService> logger) : IMemberService
    {
        private const string PhotoFolder = "Members"; // single constant
        public async Task<List<MemberViewModel>> GetAllAsync()
        {
            // Get all members
            var members = await context.Members.AsNoTracking().ToListAsync();

            // Extract member ids so we can fetch memberships only for these members
            var memberIds = members.Select(m => m.Id).ToList();

            // Get all active memberships and their related plans
            var activeMemberships = await context.MemberShips.AsNoTracking()
                .Include(ms => ms.Plan)
                .Where(ms => memberIds.Contains(ms.MemberId) &&
                             ms.EndDate >= DateTime.Now)
                .ToListAsync();

            // If a member has multiple active memberships, keep the latest one
            var membershipByMemberId = activeMemberships
                .GroupBy(ms => ms.MemberId)
                .ToDictionary(
                    g => g.Key,
                    g => g.MaxBy(ms => ms.CreatedAt) // CreatedAt is StartDate column in DB
                );

            // GetValueOrDefault returns null if the member has no active membership
            var result = members.Select(member =>
                            MapToViewModel(member,
                                    membershipByMemberId.GetValueOrDefault(member.Id)
                             )).ToList();

            return result;
        }

        public async Task<MemberViewModel?> GetByIdAsync(int id)
        {
            var member = await context.Members.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
            if (member is null)
            {
                return null;
            }

            var activeMembership = await context.MemberShips  // Get the most recent active membership for the member
                .AsNoTracking()
                .Include(ms => ms.Plan)
                .Where(ms => ms.MemberId == member.Id && ms.EndDate >= DateTime.Now)
                .OrderByDescending(ms => ms.CreatedAt) // CreatedAt = StartDate column in DB
                .FirstOrDefaultAsync();

            return MapToViewModel(member, activeMembership);
        }

        public async Task<HealthRecordViewModel?> GetHealthRecordAsync(int memberId)
        {
            var member = await context.Members
                .AsNoTracking()
                .Include(m => m.HealthRecord)
                .FirstOrDefaultAsync(m => m.Id == memberId);

            if (member?.HealthRecord is null)
            {
                return null;
            }

            return new HealthRecordViewModel
            {
                Height = member.HealthRecord.Height,
                Weight = member.HealthRecord.Weight,
                BloodType = member.HealthRecord.BloodType,
                Note = member.HealthRecord.Note
            };
        }

        public async Task<EditMemberViewModel?> GetForEditAsync(int id)
        {
            var member = await context.Members.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
            if (member is null)
            {
                return null;
            }

            return new EditMemberViewModel
            {
                Name = member.Name,
                Email = member.Email,
                Phone = member.Phone,
                BuildingNumber = member.Address.BuildingNumber,
                Street = member.Address.Street,
                City = member.Address.City,
                Photo = member.Photo
            };
        }

        public async Task<bool> CreateAsync(CreateMemberViewModel model)
        {
            try
            {
                if (await IsEmailTakenAsync(model.Email) || await IsPhoneTakenAsync(model.Phone))
                {
                    return false;
                }

                var photoName = await attachment.UploadAsync(PhotoFolder, model.PhotoFile);
                if (string.IsNullOrEmpty(photoName))
                {
                    return false;
                }

                var member = new Member
                {
                    Name = model.Name,
                    Email = model.Email,
                    Phone = model.Phone,
                    DateOfBirth = model.DateOfBirth,
                    Gender = model.Gender,
                    Photo = photoName,
                    Address = new Address
                    {
                        BuildingNumber = model.BuildingNumber,
                        Street = model.Street,
                        City = model.City
                    },
                    HealthRecord = new HealthRecord
                    {
                        Height = model.HealthRecord.Height,
                        Weight = model.HealthRecord.Weight,
                        BloodType = model.HealthRecord.BloodType,
                        Note = model.HealthRecord.Note
                    }
                };

                context.Members.Add(member);
                var saved = await context.SaveChangesAsync() > 0;

                if (!saved) // if not saved, delete the uploaded photo to avoid orphaned files
                {
                    attachment.Delete(PhotoFolder, photoName);
                }

                return saved;
            }
            catch(Exception ex)
            {
                logger.LogError(ex, "Failed to create member with email {Email}", model.Email);
                return false;
            }
        }
        public async Task<bool> EditAsync(int id, EditMemberViewModel model)
        {
            try
            {
                var member = await context.Members.FirstOrDefaultAsync(m => m.Id == id);
                if (member is null)
                {
                    return false;
                }

                if (await IsEmailTakenAsync(model.Email, excludeId: id) ||
                    await IsPhoneTakenAsync(model.Phone, excludeId: id))
                {
                    return false;
                }

                member.Name = model.Name;
                member.Email = model.Email;
                member.Phone = model.Phone;
                member.Address.BuildingNumber = model.BuildingNumber;
                member.Address.Street = model.Street;
                member.Address.City = model.City;

                // Update photo only if a new photo is provided otherwise, keep the existing one
                if (!string.IsNullOrEmpty(model.Photo))

                {
                    member.Photo = model.Photo;
                }

                member.UpdatedAt = DateTime.Now;

                return await context.SaveChangesAsync() > 0;
            }
            catch(Exception ex)
            {
                logger.LogError(ex, "Failed to edit member with id {Id}", id);
                return false;
            }
        }
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var member = await context.Members.FirstOrDefaultAsync(m => m.Id == id);
                if (member is null)
                {
                    return false;
                }

                // Block deletion if the member has any upcoming sessions as MemberSession connects members to sessions
                var hasUpcomingSessions = await context.MemberSessions
                    .Where(ms => ms.MemberId == id)
                    .AnyAsync(ms => ms.Session.StartDate > DateTime.Now);  // session start date is in the future

                if (hasUpcomingSessions)
                {
                    return false;
                }

                var memberShips = await context.MemberShips.Where(ms => ms.MemberId == id).ToListAsync();
                context.MemberShips.RemoveRange(memberShips); // remove all memberships of the member

                context.Members.Remove(member);
                var deleted = await context.SaveChangesAsync() > 0;

                if (deleted)
                {
                    attachment.Delete(PhotoFolder, member.Photo);   // delete the member's photo from storage
                }

                return deleted;
            }
            catch(Exception ex)
            {
                logger.LogError(ex, "Failed to delete member with id {Id}", id);
                return false;
            }
        }

        public async Task<List<PlanViewModel>> GetAvailablePlansAsync()
{
    // Only show active plans
    return await context.Plans
        .AsNoTracking()
        .Where(p => p.IsActive)
        .Select(p => new PlanViewModel
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            DurationDays = p.DurationDays,
            Price = p.Price,
            IsActive = p.IsActive
        })
        .ToListAsync();
}

public async Task<List<SessionViewModel>> GetAvailableSessionsAsync()
{
    var now = DateTime.Now;

    // Load upcoming sessions with Trainer and Category in one query
    var sessions = await context.Sessions
        .AsNoTracking()
        .Include(s => s.SessionTrainer)
        .Include(s => s.SessionCategory)
        .Where(s => s.StartDate > now)
        .ToListAsync();

    var sessionIds = sessions.Select(s => s.Id).ToList();

    var bookedCounts = await context.MemberSessions
        .Where(ms => sessionIds.Contains(ms.SessionId))
        .GroupBy(ms => ms.SessionId)
        .Select(g => new { SessionId = g.Key, Count = g.Count() })
        .ToDictionaryAsync(x => x.SessionId, x => x.Count);

    // Filter out full sessions and map to ViewModels in memory
    return sessions
        .Select(s => new
        {
            Session = s,
            BookedCount = bookedCounts.GetValueOrDefault(s.Id)
        })
        .Where(x => x.BookedCount < x.Session.Capacity) // only sessions with slots
        .Select(x => new SessionViewModel
        {
            Id = x.Session.Id,
            CategoryName = x.Session.SessionCategory.CategoryName,
            TrainerName = x.Session.SessionTrainer.Name,
            Description = x.Session.Description,
            StartDate = x.Session.StartDate,
            EndDate = x.Session.EndDate,
            Capacity = x.Session.Capacity,
            AvailableSlots = x.Session.Capacity - x.BookedCount
        })
        .ToList();
}

public async Task<bool> AssignPlanAsync(int memberId, int planId)
{
    try
    {
        // Confirm member and plan both exist and plan is active
        var member = await context.Members.FindAsync(memberId);
        var plan = await context.Plans.FindAsync(planId);
        if (member is null || plan is null || !plan.IsActive)
            return false;

        var membership = new MemberShip
        {
            MemberId = memberId,
            PlanId = planId,
            // EndDate calculated from plan duration starting today
            EndDate = DateTime.Now.AddDays(plan.DurationDays)
        };

        context.MemberShips.Add(membership);
        return await context.SaveChangesAsync() > 0;
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Failed to assign plan {PlanId} to member {MemberId}", planId, memberId);
        return false;
    }
}

public async Task<bool> BookSessionAsync(int memberId, int sessionId)
{
    try
    {
        var member = await context.Members.FindAsync(memberId);
        var session = await context.Sessions.FindAsync(sessionId);
        if (member is null || session is null) return false;

        // Session must be upcoming — can't book a started/completed session
        if (session.StartDate <= DateTime.Now) return false;

        // Check member hasn't already booked this session
        var alreadyBooked = await context.MemberSessions
            .AnyAsync(ms => ms.MemberId == memberId && ms.SessionId == sessionId);
        if (alreadyBooked) return false;

        // Check session still has capacity
        var bookedCount = await context.MemberSessions
            .CountAsync(ms => ms.SessionId == sessionId);
        if (bookedCount >= session.Capacity) return false;

        var memberSession = new MemberSession
        {
            MemberId = memberId,
            SessionId = sessionId,
            IsAttend = false // attendance marked later
        };

        context.MemberSessions.Add(memberSession);
        return await context.SaveChangesAsync() > 0;
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Failed to book session {SessionId} for member {MemberId}", sessionId, memberId);
        return false;
    }
}

        #region Helper Methods

        private static MemberViewModel MapToViewModel(Member member, MemberShip? activeMembership)
        {
            var vm = new MemberViewModel
            {
                Id = member.Id,
                Name = member.Name,
                Photo = member.Photo,
                Email = member.Email,
                Phone = member.Phone,
                Gender = member.Gender.ToString(),
                DateOfBirth = member.DateOfBirth.ToString("yyyy-MM-dd"),
                Address = $"{member.Address.BuildingNumber}, {member.Address.Street}, {member.Address.City}"
            };

            if (activeMembership is not null)
            {
                vm.PlanName = activeMembership.Plan.Name;
                vm.MemberShipStartDate = activeMembership.CreatedAt.ToShortDateString();
                vm.MemberShipEndDate = activeMembership.EndDate.ToShortDateString();
            }

            return vm;
        }

        private async Task<bool> IsEmailTakenAsync(string email, int? excludeId = null)
        {
            return await context.Members.AnyAsync(m => m.Email == email && m.Id != excludeId);
        }

        private async Task<bool> IsPhoneTakenAsync(string phone, int? excludeId = null)
        {
            return await context.Members.AnyAsync(m => m.Phone == phone && m.Id != excludeId);
        }

        #endregion
    }
}