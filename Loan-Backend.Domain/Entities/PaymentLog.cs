using Loan_Backend.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loan_Backend.Domain.Entities
{
    public class PaymentLog : BaseEntity<Guid>
    {
        public Guid LoanId { get; private set; }
        public decimal AmountPaid { get; private set; }
        public string CurrencyCode { get; private set; } = string.Empty;
        public DateTime DateLogged { get; private set; }
        public string LoggedBy { get; private set; } = string.Empty;
        public DateTime Approved { get; private set; }
        public string ApprovedBy { get; private set; } = string.Empty;
        public string Status { get; private set; } = string.Empty;

        public PaymentLog(Guid id):base(id)
        {
                
        }

        public PaymentLog():base(Guid.NewGuid())
        {
                
        }

        public static PaymentLog Create(Guid loanId, decimal amount, string currencyCode, string loggedBy)
        {
            return new PaymentLog
            {
                AmountPaid = amount,
                LoanId = loanId,
                CurrencyCode = currencyCode,
                DateLogged = DateTime.UtcNow.AddHours(1),
                LoggedBy = loggedBy.ToProperCase(),
                Status = PaymentStatusEnum.Pending_Approver_Review.ToString()
            };
        }

        public void Approve(string approvedBy)
        {
            ApprovedBy = approvedBy.ToProperCase();
        }

        public void Decline(string declinedBy)
        {
            ApprovedBy = declinedBy.ToProperCase();
            Status = PaymentStatusEnum.Declined.ToString();
        }
    }
}
