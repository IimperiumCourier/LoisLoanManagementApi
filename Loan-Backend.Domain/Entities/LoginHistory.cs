using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loan_Backend.Domain.Entities
{
    public class LoginHistory : BaseEntity<Guid>
    {
        public Guid AdminId { get; private set; }
        public bool IsLoginSuccessful { get; private set; }
        public DateTime DateCreated { get; private set; }

        public LoginHistory(Guid id):base(id)
        {
                
        }

        public LoginHistory():base(Guid.NewGuid())
        {
                
        }

        internal static LoginHistory Create(Guid adminId, bool isLoginSuccessful)
        {
            return new LoginHistory
            {
                AdminId = adminId,
                DateCreated = DateTime.UtcNow.AddHours(1),
                IsLoginSuccessful = isLoginSuccessful
            };
        }
    }
}
