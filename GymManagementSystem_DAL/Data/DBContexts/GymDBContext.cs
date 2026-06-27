using GymManagementSystem_DAL.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace GymManagementSystem_DAL.Data.DBContexts
{
    public class GymDBContext(DbContextOptions<GymDBContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        override protected void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Applies every IEntityTypeConfiguration<T> class in this assembly
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            // ApplicationUser is configured here directly
            modelBuilder.Entity<ApplicationUser>(Eb => 
            {
                Eb.Property(X => X.FirstName)
                  .HasColumnType("varchar")
                  .HasMaxLength(50);
                Eb.Property(X => X.LastName)
                  .HasColumnType("varchar")
                  .HasMaxLength(50);
            });
        }

        #region Db Sets
        public DbSet<Member> Members { get; set; } = null!;
        public DbSet<Trainer> Trainers { get; set; } = null!;
        public DbSet<Plan> Plans { get; set; } = null!;
        public DbSet<MemberShip> MemberShips { get; set; } = null!;
        public DbSet<Session> Sessions { get; set; } = null!;
        public DbSet<MemberSession> MemberSessions { get; set; } = null!;
        public DbSet<HealthRecord> HealthRecords { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        #endregion
    }
}
