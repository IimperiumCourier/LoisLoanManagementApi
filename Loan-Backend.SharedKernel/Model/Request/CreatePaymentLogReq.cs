using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loan_Backend.SharedKernel.Model.Request
{
    public class CreatePaymentLogReq
    {
        [Required(ErrorMessage ="Loan Id is required")]
        public Guid LoanId { get; set; }
        public decimal Amount { get; set; }
        public string CurrencyCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Repayment Id is required")]

        public Guid RepaymentId { get; set; }
    }
}
