using GymManagementSystem_DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem_DAL.Data.Configurations
{
    public class SessionConfigurations : IEntityTypeConfiguration<Session>
    {
        public void Configure(EntityTypeBuilder<Session> builder)
        {
            builder.ToTable(Tb =>
            {
                Tb.HasCheckConstraint("SessionCapacityCheck", "Capacity between 1 and 25");
                Tb.HasCheckConstraint("SessionEndDateCheck", "EndDate > StartDate");
            });

            // One Category -> Many Sessions
            builder.HasOne(x => x.SessionCategory)
                   .WithMany(x => x.Sessions)
                   .HasForeignKey(x => x.CategoryId);

            // One Trainer -> Many Sessions
            builder.HasOne(x => x.SessionTrainer)
                   .WithMany(x => x.TrainerSessions)
                   .HasForeignKey(x => x.TrainerId);
        }
    }
}
