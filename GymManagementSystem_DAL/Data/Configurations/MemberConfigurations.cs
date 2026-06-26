using GymManagementSystem_DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem_DAL.Data.Configurations
{
    public class MemberConfigurations : GymUserConfirgurations<Member>, IEntityTypeConfiguration<Member>
    {
        public new void Configure(EntityTypeBuilder<Member> builder)
        {
            base.Configure(builder); // applies all the shared GymUser rules first
            builder.ToTable("Members");
            builder.Property(x => x.CreatedAt)
                   .HasColumnName("JoinDate")       
                   .HasDefaultValueSql("GETDATE()"); 
        }
    }
}
