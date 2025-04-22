using Loan_Backend.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loan_Backend.Domain.Entities
{
    public class Permission : BaseEntity<Guid>
    {
        public string Name { get; private set; } = string.Empty;
        public DateTime CreatedAt { get; private set; }
        public DateTime LastDateUpdated { get; private set; }
        public bool IsActive { get; private set; }

        public Permission(Guid id):base(id)
        {
                
        }

        public Permission():base(Guid.NewGuid())
        {
                
        }

        public static Permission Create(string name)
        {
            return new Permission
            {
                Name = name.ToTrimmedAndLowerCase(),
                CreatedAt = DateTime.UtcNow.AddHours(1),
                IsActive = true
            };
        }

        public void Deactivate()
        {
            IsActive = false;
            LastDateUpdated = DateTime.UtcNow.AddHours(1);
        }

        public void Activate()
        {
            IsActive = true;
            LastDateUpdated = DateTime.UtcNow.AddHours(1);
        }

        public List<RolePermission> CreateRolePermission(List<Guid> roleIds, Guid grantorId)
        {
            return [.. roleIds.Select(roleId => RolePermission.Create(roleId, Id, grantorId))];
        }
    }
}
