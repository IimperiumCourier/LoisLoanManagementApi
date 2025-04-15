using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loan_Backend.SharedKernel
{
    public enum LoanStatusEnum
    {
        Approved,
        Declined,
        Pending_Approver_Review,
        Defaulted,
        Paid
    }
}
