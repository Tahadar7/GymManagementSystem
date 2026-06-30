using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem_DAL.Entities
{
    // Join entity connecting a Member to a Plan.
    public class MemberShip : BaseEntity
    {
        public DateTime EndDate { get; set; }
        public string Status  // computed value not stored in DB so no setter needed
        {
            get
            {
                if (EndDate >= DateTime.Now)
                {
                    return "Active";
                }
                else
                {
                    return "Expired";
                }
            }
        }

        public int MemberId { get; set; }
        public Member Member { get; set; } = null!;
        public int PlanId { get; set; }
        public Plan Plan { get; set; } = null!;
    }
}
