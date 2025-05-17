using Loan_Backend.SharedKernel;
using Loan_Backend.SharedKernel.Model.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loan_Backend.Domain.Entities
{
    public class CustomerLoan : BaseEntity<Guid>
    {
        public Guid CustomerId { get; private set; }
        public string LoanGroup { get; private set; } = string.Empty;
        public decimal Amount { get; private set; }
        public string CurrencyCode { get; private set; } = string.Empty;
        public string CreatedBy { get; private set; } = string.Empty;
        public DateTime CreatedDate { get; private set; }
        public string ApprovedBy { get; private set; } = string.Empty;
        public DateTime ApprovedDate { get; private set; }
        public string Status { get; private set; } = string.Empty;
        public decimal InterestRatePercent { get; private set; }
        public decimal RepaymentAmount { get; private set; }
        public int DurationInWeeks { get; private set; }

        public CustomerLoan(Guid id):base(id)
        {
                
        }

        public CustomerLoan():base(Guid.NewGuid())
        {
                
        }

        internal static CustomerLoan Create(Guid customerId, string createdBy,
                                            LoanPreference loanPreference)
        {
            return new CustomerLoan
            {
                Amount = loanPreference.Amount,
                ApprovedBy = string.Empty,
                ApprovedDate = DateTime.MinValue,
                CreatedDate = DateTime.UtcNow.AddHours(1),
                CreatedBy = createdBy,
                CurrencyCode = loanPreference.CurrencyCode,
                CustomerId = customerId,
                LoanGroup = loanPreference.LoanGroup,
                Status = LoanStatusEnum.Pending_Approver_Review.ToString(),
                InterestRatePercent = loanPreference.InterestRate,
                DurationInWeeks = loanPreference.DurationInWeeks
            };
        }

        public void AddApproverInfo(string approver)
        {
            ApprovedBy = approver.ToProperCase();
            ApprovedDate = DateTime.UtcNow.AddHours(1);
            Status = LoanStatusEnum.Approved.ToString();
        }

        public void Defaulted()
        {
            Status = LoanStatusEnum.Defaulted.ToString();
        }

        public void Declined()
        {
            Status = LoanStatusEnum.Declined.ToString();
        }

        public void AddRepaymentAmount(decimal repaymentAmount)
        {
            RepaymentAmount = repaymentAmount;
        }

        public bool IsPaymentCompleted(List<PaymentLog> paymentLogs)
        {
            if(paymentLogs == null)
            {
                return false;
            }

            var amountPaid = paymentLogs.Sum(e => e.AmountPaid);
            if(RepaymentAmount > amountPaid)
            {
                return false;
            }

            Status = LoanStatusEnum.Paid.ToString();
            return true;
        }

        public decimal InterestEarned => RepaymentAmount - Amount;
    }
}
