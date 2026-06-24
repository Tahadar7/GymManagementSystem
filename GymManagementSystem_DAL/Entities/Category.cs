using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem_DAL.Entities
{
    public class Category : BaseEntity
    {
        public string CategoryName { get; set; } = null!;

        // One-to-many: a Category can have many Sessions
        public ICollection<Session> Sessions { get; set; } = null!;
    }
}
