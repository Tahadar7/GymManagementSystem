using GymManagementSystem_DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem_DAL.Data.Configurations
{
    public class GymUserConfirgurations<T> : IEntityTypeConfiguration<T> where T : GymUser
    {
        public void Configure(EntityTypeBuilder<T> builder)
        {
            builder.Property(X => X.Name).HasColumnType("varchar").HasMaxLength(50);
            builder.Property(X => X.Email).HasColumnType("varchar").HasMaxLength(100);
            builder.Property(X => X.Phone).HasColumnType("varchar").HasMaxLength(11);

            // SQL CHECK constraints
            builder.ToTable(Tb =>
            {
                Tb.HasCheckConstraint("GymUserValidEmailCheck", "Email LIKE '_%@_%._%'");
                Tb.HasCheckConstraint("GymUserValidPhoneCheck", "Phone LIKE '03%' and Phone Not Like '%[^0-9]%'");
            });

            // Unique indexes at DB Level
            builder.HasIndex(x => x.Email).IsUnique();
            builder.HasIndex(x => x.Phone).IsUnique();

            // Address is [Owned] in GymUser Table
            builder.OwnsOne(x => x.Address, AddressBuilder =>
            {
                AddressBuilder.Property(a => a.BuildingNumber).HasColumnName("BuildingNumber");
                
                AddressBuilder.Property(a => a.Street).HasColumnName("Street")
                .HasColumnType("varchar")
                .HasMaxLength(30);

                AddressBuilder.Property(a => a.City)
                .HasColumnType("varchar")
                .HasMaxLength(30);
            });
        }
    }
}
