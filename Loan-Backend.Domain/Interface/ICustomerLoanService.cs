using Loan_Backend.Domain.Entities;
using Loan_Backend.SharedKernel;
using Loan_Backend.SharedKernel.Model.DTO;
using Loan_Backend.SharedKernel.Model.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loan_Backend.Domain.Interface
{
    public interface ICustomerLoanService
    {
        Task<ResponseWrapper<PagedResult<CustomerLoan>>> GetLoanByCustomerId(Guid customerId, LoanStatusEnum? status,
                                                                                InterestFrequencyEnum? type, int pageNum = Common.PageNumber, int pageSize = Common.PageSize);
        Task<ResponseWrapper<CustomerLoan>> GetLoanById(Guid customerLoanId);
        Task<ResponseWrapper<string>> ApproveLoan(Guid customerLoanId, string approver);
        Task<ResponseWrapper<CustomerLoan>> CreateLoan(CreateLoanReq request, string createdBy);
        Task<ResponseWrapper<string>> DeclineLoan(Guid customerLoanId);
        Task<ResponseWrapper<string>> DefaultLoan(Guid customerLoanId);
        Task<ResponseWrapper<PagedResult<CustomerLoan>>> GetLoans(LoanStatusEnum? status,
                                                                  InterestFrequencyEnum? type,
                                                                  int pageNum = 1, int pageSize = 10);
    }
}
