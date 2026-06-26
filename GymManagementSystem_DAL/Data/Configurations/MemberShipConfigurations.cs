using GymManagementSystem_DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem_DAL.Data.Configurations
{
    public class MemberShipConfigurations : IEntityTypeConfiguration<MemberShip>
    {
        public void Configure(EntityTypeBuilder<MemberShip> builder)
        {
            builder.Property(X => X.CreatedAt)  // CreatedAt is StartDate in DB
                   .HasColumnName("StartDate")
                   .HasDefaultValueSql("GETDATE()");

            // Composite PK
            builder.HasKey(X => new { X.MemberId, X.PlanId });
            builder.Ignore(X => X.Id);

            // Status is computed only getter no setter.
            builder.Ignore(X => X.Status);
        }
    }
}
