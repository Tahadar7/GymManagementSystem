using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem_DAL.Entities
{
    public class Session : BaseEntity
    {
        public string Description { get; set; } = null!;
        public int Capacity { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        #region RelationShip

        #region Category - Session
        // Many-to-one: a Session belongs to one Category
        public int CategoryId { get; set; }
        public Category SessionCategory { get; set; } = null!;
        #endregion

        #region Trainer - Session
        // Many-to-one: a Session is led by one Trainer
        public int TrainerId { get; set; }
        public Trainer SessionTrainer { get; set; } = null!; 
        #endregion

        #region MemberSession - Session
        // Many-to-many
        // a Session have many attending Members
        public ICollection<MemberSession> SessionMembers { get; set; } = null!;
        #endregion

        #endregion
    }
}
