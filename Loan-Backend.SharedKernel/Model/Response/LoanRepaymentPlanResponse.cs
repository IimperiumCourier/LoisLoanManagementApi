using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loan_Backend.SharedKernel.Model.Response
{
    public class LoanRepaymentPlanResponse
    {
        public Guid Id { get; set; }
        public Guid LoanId { get; set; }
        public string Fullname { get; set; } = string.Empty;
        public decimal AmountBorrowed { get; set; }
        public decimal AmountToBeRepaid { get; set; }
        public decimal AmountPerInstallment{get;set;}
        public bool IsPaid {  get; set; }
        public DateTime DueDate { get; set; }
    }
}
