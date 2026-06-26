using Microsoft.EntityFrameworkCore;
using GymManagementSystem_DAL.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem_DAL.Data.Configurations
{
    public class CategoryConfigurations : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            //  column type (varchar(20))
            builder.Property(x => x.CategoryName)
                   .HasColumnType("varchar")
                   .HasMaxLength(20);
        }
    }
}
