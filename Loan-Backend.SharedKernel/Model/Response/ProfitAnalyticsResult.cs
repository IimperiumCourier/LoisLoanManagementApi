using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loan_Backend.SharedKernel.Model.Response
{
    public class ProfitAnalyticsResult
    {
        public string Period { get; set; }  = string.Empty;
        public decimal TotalDisbursement { get; set; }
        public decimal TotalRepayment { get; set; }
        public decimal InterestEarned { get; set; }
        public decimal OverdueLoans { get; set; }
        public decimal TotalOverdueAmount { get; set; }
        public decimal Profit { get; set; }
    }
}
