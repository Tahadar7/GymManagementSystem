using GymManagementSystem_DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem_DAL.Data.Configurations
{
    public class HealthRecordConfigurations : IEntityTypeConfiguration<HealthRecord>
    {
        public void Configure(EntityTypeBuilder<HealthRecord> builder)
        {
            // HealthRecord is a column in Member Table
            builder.ToTable("Members")
                   .HasKey(x => x.Id); 

            builder.HasOne<Member>()
                    .WithOne(x => x.HealthRecord)
                     .HasForeignKey<HealthRecord>(x => x.Id); // HealthRecord.Id IS Member.Id (shared key)

            builder.Ignore(X => X.CreatedAt);  // Member as its own these two columns already
            builder.Ignore(X => X.UpdatedAt);
        }
    }
}
