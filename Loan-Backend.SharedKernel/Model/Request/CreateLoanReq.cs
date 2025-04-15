using Loan_Backend.SharedKernel.Model.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loan_Backend.SharedKernel.Model.Request
{
    public class CreateLoanReq
    {
        public Guid CustomerId { get; set; }
        public LoanPreference? LoanPreference { get; set; }
    }
}
