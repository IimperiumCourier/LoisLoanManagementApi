using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loan_Backend.Domain.Entities
{
    public class RolePermission : BaseEntity<Guid>
    {
        public Guid RoleId { get; private set; }
        public Guid PermissionId { get; private set; }
        public Guid Grantor { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime DateGranted { get; private set; }
        public DateTime LastDateUpdated { get; private set; }

        public RolePermission(Guid id):base(id)
        {
                
        }

        public RolePermission() : base(Guid.NewGuid())
        {

        }

        public static RolePermission Create(Guid roleId, Guid permissionId, Guid grantorId)
        {
            return new RolePermission
            {
                RoleId = roleId,
                DateGranted = DateTime.UtcNow.AddHours(1),
                Grantor = grantorId,
                IsActive = true,
                PermissionId = permissionId
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
