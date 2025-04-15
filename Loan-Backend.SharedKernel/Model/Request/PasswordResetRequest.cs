using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loan_Backend.SharedKernel.Model.Request
{
    public class PasswordResetRequest : PasswordChangeRequest
    {
        public Guid Id { get; set; }
    }
}
