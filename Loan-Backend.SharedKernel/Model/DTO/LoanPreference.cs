using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loan_Backend.SharedKernel.Model.DTO
{
    public class LoanPreference
    {
        public string LoanGroup { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string CurrencyCode { get; set; } = string.Empty;
        public decimal InterestRate { get; set; }
        public int DurationInWeeks { get; set; }
    }
}
