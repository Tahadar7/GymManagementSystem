using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem_BLL.ViewModels.MemberViewModels
{
        public class MemberViewModel
        {
            public int Id { get; set; }
            public string Name { get; set; } = null!;
            public string? Photo { get; set; } 
            public string Email { get; set; } = null!;
            public string Phone { get; set; } = null!;
            public string Gender { get; set; } = null!; // enum displayed as text
            public string DateOfBirth { get; set; } = null!;
            public string Address { get; set; } = null!; // BuildingNumber + Street + City
            public string? PlanName { get; set; }
            public string? MemberShipStartDate { get; set; }
            public string? MemberShipEndDate { get; set; }
        }
}
