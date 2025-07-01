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
        public Guid RepaymentId { get; private set; }

        public PaymentLog(Guid id) : base(id)
        {
            // This constructor is used when an ID is explicitly passed (e.g., when retrieving from DB)
        }

        public PaymentLog() : base(Guid.NewGuid())
        {
            // This parameterless constructor automatically generates a new GUID for the base entity
        }

        public static PaymentLog Create(Guid loanId, decimal amount, string currencyCode, string loggedBy, Guid repaymentId)
        {
            // Explicitly call the parameterless constructor to ensure a new GUID is generated
            // by the BaseEntity constructor.
            // Then, set the other properties using object initializer syntax.
            return new PaymentLog() // This calls the parameterless constructor, which calls base(Guid.NewGuid())
            {
                AmountPaid = amount,
                LoanId = loanId,
                CurrencyCode = currencyCode,
                DateLogged = DateTime.UtcNow.AddHours(1),
                LoggedBy = loggedBy.ToProperCase(),
                Status = PaymentStatusEnum.Pending_Approver_Review.ToString(),
                RepaymentId = repaymentId
            };
        }

        public void Approve(string approvedBy)
        {
            ApprovedBy = approvedBy.ToProperCase();
            Status = PaymentStatusEnum.Approved.ToString();
        }

        public void Decline(string declinedBy)
        {
            ApprovedBy = declinedBy.ToProperCase();
            Status = PaymentStatusEnum.Declined.ToString();
        }
    }
}