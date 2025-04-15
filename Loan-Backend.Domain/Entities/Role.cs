using Loan_Backend.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loan_Backend.Domain.Entities
{
    public class Role : BaseEntity<Guid>
    {
        public string Name { get; private set; } = string.Empty;
        public DateTime CreatedAt { get; private set; }
        public DateTime LastDateUpdated { get; private set; }
        public bool IsActive { get; private set; }

        public Role(Guid id) : base(id)
        {

        }

        public Role() : base(Guid.NewGuid())
        {

        }

        public static Role Create(string name)
        {
            return new Role
            {
                Name = name.ToProperCase(),
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
    }
}
