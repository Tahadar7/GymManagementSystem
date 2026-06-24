using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem_DAL.Entities
{
    // Join entity connecting a Member to a Session.
    public class MemberSession : BaseEntity
    {
        public bool IsAttend { get; set; } // did the member actually attend
        public int MemberId { get; set; }
        public Member Member { get; set; } = null!;

        public int SessionId { get; set; }
        public Session Session { get; set; } = null!;
    }
}
