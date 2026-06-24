using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem_DAL.Entities
{
    public class Member : GymUser
    {
        public string Photo { get; set; } = null!;

        #region RelationShip

        #region Member - HealthRecord
        // One-to-one: each Member has exactly one HealthRecord
        public HealthRecord HealthRecord { get; set; } = null!;
        #endregion

        #region Member - MemberShip
        // One-to-many: a Member can have multiple MemberShips
        public ICollection<MemberShip> MemberShips { get; set; } = null!;
        #endregion

        #region Member - Session
        // Many-to-many
        // a Member can attend many Sessions
        public ICollection<MemberSession> MemberSessions { get; set; } = null!;
        #endregion

        #endregion
    }
}
