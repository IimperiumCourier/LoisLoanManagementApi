using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loan_Backend.Domain.Entities
{
    public class AdminRole : BaseEntity<Guid>
    {
        public Guid AdminId { get; private set; }
        public Guid RoleId { get; private set; }
        public Guid Grantor { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime DateGranted { get; private set; }
        public DateTime LastDateUpdated { get; private set; }

        public AdminRole(Guid id) : base(id)
        {

        }

        public AdminRole() : base(Guid.NewGuid())
        {

        }

        public static AdminRole Create(Guid adminId, Guid roleId, Guid grantorId)
        {
            return new AdminRole
            {
                AdminId = adminId,
                DateGranted = DateTime.UtcNow.AddHours(1),
                Grantor = grantorId,
                IsActive = true,
                RoleId = roleId
            };
        }
        public void Deacivate()
        {
            IsActive = false;
            LastDateUpdated = DateTime.UtcNow.AddHours(1);
        }

        public void Acivate()
        {
            IsActive = true;
            LastDateUpdated = DateTime.UtcNow.AddHours(1);
        }
    }
}
