using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loan_Backend.Domain.Entities
{
    public class CustomerLoanRepaymentPlan : BaseEntity<Guid>
    {
        public DateTime DueDate { get; private set; }
        public bool IsPaid {  get; private set; }
        public DateTime DatePaid { get; private set; }
        public Guid LoanId { get; set; }
        public DateTime DateCreated { get; private set; }

        public static CustomerLoanRepaymentPlan CreateRepaymentPlan(DateTime dueDate, Guid loanId) => new CustomerLoanRepaymentPlan
        {
            DatePaid = DateTime.MinValue,
            DueDate = dueDate,
            LoanId = loanId,
            DateCreated = DateTime.UtcNow.AddHours(1)
        };

        public void MarkRepaymentAsPaid()
        {
            IsPaid = true;
            DatePaid = DateTime.UtcNow.AddHours(1);
        }
    }
}
