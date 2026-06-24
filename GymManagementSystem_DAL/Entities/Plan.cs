using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem_DAL.Entities
{
    // A membership plan offered by the gym
    public class Plan : BaseEntity
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int DurationDays { get; set; }
        public decimal Price { get; set; }
        public bool IsActive { get; set; } 

        // One-to-many: many Members can subscribe the same Plan
        public ICollection<MemberShip> PlanMembers { get; set; } = null!;
    }
}
