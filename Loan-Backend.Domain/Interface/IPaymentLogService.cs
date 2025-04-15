using Loan_Backend.Domain.Entities;
using Loan_Backend.SharedKernel;
using Loan_Backend.SharedKernel.Model.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loan_Backend.Domain.Interface
{
    public interface IPaymentLogService
    {
        Task<ResponseWrapper<PaymentLog>> LogPayment(Guid loanId, decimal amount, string currencyCode, string loggedBy);
        Task<ResponseWrapper<PagedResult<PaymentLog>>> GetPaymentLogUsingLoanId(Guid loanId, int pageNum = Common.PageNumber, int pageSize = Common.PageSize);
        Task<ResponseWrapper<PagedResult<PaymentLog>>> GetPaymentLogsPendingApproval(int pageNum = Common.PageNumber, int pageSize = Common.PageSize);
        Task<ResponseWrapper<string>> ApprovePayment(Guid paymentLogId, string approver);
        Task<ResponseWrapper<string>> DeclinePayment(Guid paymentLogId, string approver);

    }
}
