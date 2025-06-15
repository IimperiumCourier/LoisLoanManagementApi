using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loan_Backend.SharedKernel.Model.DTO
{
    public class RepaymentPlanDto
    {
        public List<DateTime> DueDates { get; set; }
        public int NumberOfPaymentInstallments { get; set; }
        public decimal AmountPerInstallment {  get; set; }
    }
}
