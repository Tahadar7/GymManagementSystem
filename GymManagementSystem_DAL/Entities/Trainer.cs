using GymManagementSystem_DAL.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using static System.Collections.Specialized.BitVector32;

namespace GymManagementSystem_DAL.Entities
{
    public class Trainer : GymUser
    {
        public Specialties Specialties { get; set; }

        // One-to-many: a Trainer leads many Sessions
        public ICollection<Session> TrainerSessions { get; set; } = null!;
    }
}
