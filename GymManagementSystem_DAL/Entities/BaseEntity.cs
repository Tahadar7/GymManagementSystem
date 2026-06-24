using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem_DAL.Entities
{
    // Base class for all entities
    public abstract class BaseEntity
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
