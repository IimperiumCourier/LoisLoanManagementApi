using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loan_Backend.SharedKernel.Model.DTO
{
    public class PaymentLogDto
    {
        public Guid PaymentLogId { get; init; }
        public Guid LoanId { get; init; }
        public decimal AmountPaid { get; init; }
        public string CurrencyCode { get; init; }
        public DateTime DateLogged { get; init; }
        public string LoggedBy { get; init; }
        public DateTime Approved { get; init; }
        public string ApprovedBy { get; init; }
        public string Status { get; init; } 
        public string CustomerId { get; init; }
        public string CustomerName { get; init; }
        public string CustomerEmail {  get; init; }
        public string CustomerPhone { get; init; }
    }
}
