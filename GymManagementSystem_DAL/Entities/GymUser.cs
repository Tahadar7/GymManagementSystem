using GymManagementSystem_DAL.Entities.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementSystem_DAL.Entities
{
        // Base class for any person in the system (Member, Trainer)
        public class GymUser : BaseEntity
        {
            public string Name { get; set; } = null!;
            public string Email { get; set; } = null!;
            public string Phone { get; set; } = null!;
            public DateOnly DateOfBirth { get; set; }
            public Gender Gender { get; set; }
            public Address Address { get; set; } = null!; //  Address is just a group of fields
    }

        // [Owned] tells EF Core this is a value object, not a separate table with its own PK so its columns stored in its parent table.
        [Owned]
        public class Address
        {
            public int BuildingNumber { get; set; }
            public string Street { get; set; } = null!;
            public string City { get; set; } = null!;
        }
    }
}
