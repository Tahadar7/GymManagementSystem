using GymManagementSystem_BLL.Interfaces;
using GymManagementSystem_BLL.ViewModels.TrainerViewModels;
using GymManagementSystem_DAL.Data.DBContexts;
using GymManagementSystem_DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GymManagementSystem_BLL.Services
{
    public class TrainerService(GymDBContext context, ILogger<TrainerService> logger) : ITrainerService
    {
        public async Task<List<TrainerViewModel>> GetAllAsync()
        {
            var trainers = await context.Trainers.AsNoTracking().ToListAsync();
            return trainers.Select(MapToViewModel).ToList();
        }

        public async Task<TrainerViewModel?> GetByIdAsync(int id)
        {
            var trainer = await context.Trainers.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);
            if(trainer is null)
            {
                return null;
            }

            return MapToViewModel(trainer);
        }

        public async Task<EditTrainerViewModel?> GetForEditAsync(int id)
        {
            var trainer = await context.Trainers.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);
            if (trainer is null)
            {
                return null;
            }

            return new EditTrainerViewModel
            {
                Name = trainer.Name,
                Email = trainer.Email,
                Phone = trainer.Phone,
                BuildingNumber = trainer.Address.BuildingNumber,
                Street = trainer.Address.Street,
                City = trainer.Address.City,
                Specialties = trainer.Specialties
            };
        }

        public async Task<bool> CreateAsync(CreateTrainerViewModel model)
        {
            try
            {
                if (await IsEmailTakenAsync(model.Email) || await IsPhoneTakenAsync(model.Phone))
                {
                    return false;
                }

                var trainer = new Trainer
                {
                    Name = model.Name,
                    Email = model.Email,
                    Phone = model.Phone,
                    DateOfBirth = model.DateOfBirth,
                    Gender = model.Gender,
                    Specialties = model.Specialties,
                    Address = new Address
                    {
                        BuildingNumber = model.BuildingNumber,
                        Street = model.Street,
                        City = model.City
                    }
                };

                context.Trainers.Add(trainer);
                return await context.SaveChangesAsync() > 0;
            }
            catch(Exception ex) 
            {
                    logger.LogError(ex, "Failed to create trainer with email {Email}", model.Email);
                    return false;
            }
        }

        public async Task<bool> EditAsync(int id, EditTrainerViewModel model)
        {
            try
            {
                var trainer = await context.Trainers.FirstOrDefaultAsync(t => t.Id == id);
                if (trainer is null)
                {
                    return false;
                }

                if (await IsEmailTakenAsync(model.Email, excludeId: id) ||
                    await IsPhoneTakenAsync(model.Phone, excludeId: id))
                {
                    return false;
                }

                trainer.Name = model.Name;
                trainer.Email = model.Email;
                trainer.Phone = model.Phone;
                trainer.Address.BuildingNumber = model.BuildingNumber;
                trainer.Address.Street = model.Street;
                trainer.Address.City = model.City;
                trainer.Specialties = model.Specialties;
                trainer.UpdatedAt = DateTime.Now;

                return await context.SaveChangesAsync() > 0;
            }
            catch( Exception ex ) 
            {
                logger.LogError(ex, "Failed to edit trainer with Id {TrainerId}", id);
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var trainer = await context.Trainers.FirstOrDefaultAsync(t => t.Id == id);
                if (trainer is null)
                {
                    return false;
                }

                // Block deleting a trainer who still has sessions scheduled in the future
                if (await HasUpcomingSessionsAsync(id))
                {
                    return false;
                }

                context.Trainers.Remove(trainer);
                return await context.SaveChangesAsync() > 0;
            }
            catch(Exception ex) 
            {
                logger.LogError(ex, "Failed to delete trainer with Id {TrainerId}", id);
                return false;
            }
        }

        #region Helper Methods
        private static TrainerViewModel MapToViewModel(Trainer trainer)
        {
            return new TrainerViewModel
            {
                Id = trainer.Id,
                Name = trainer.Name,
                Email = trainer.Email,
                Phone = trainer.Phone,
                DateOfBirth = trainer.DateOfBirth.ToString("yyyy-MM-dd"),
                Gender = trainer.Gender.ToString(),
                Address = $"{trainer.Address.BuildingNumber}, {trainer.Address.Street}, {trainer.Address.City}",
                Specialties = trainer.Specialties.ToString()
            };
        }

        private async Task<bool> IsEmailTakenAsync(string email, int? excludeId = null)
        {
            return await context.Trainers.AnyAsync(t => t.Email == email && t.Id != excludeId);
        }

        private async Task<bool> IsPhoneTakenAsync(string phone, int? excludeId = null)
        {
            return await context.Trainers.AnyAsync(t => t.Phone == phone && t.Id != excludeId);
        }

        private async Task<bool> HasUpcomingSessionsAsync(int trainerId)
        {
            return await context.Sessions.AnyAsync(s => s.TrainerId == trainerId && s.StartDate > DateTime.Now);
        }

        #endregion
    }
}