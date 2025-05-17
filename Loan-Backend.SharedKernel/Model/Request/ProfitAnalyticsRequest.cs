using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loan_Backend.SharedKernel.Model.Request
{
    public class ProfitAnalyticsRequest
    {
        public ProfitAnalyticsEnum Filter { get; set; }
        public int? Month { get; set; }
        public int? Year { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public int? Quarter {  get; set; }
    }
}
