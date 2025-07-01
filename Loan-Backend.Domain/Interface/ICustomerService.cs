using Loan_Backend.Domain.Entities;
using Loan_Backend.SharedKernel;
using Loan_Backend.SharedKernel.Model.DTO;
using Loan_Backend.SharedKernel.Model.Request;
using Loan_Backend.SharedKernel.Model.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loan_Backend.Domain.Interface
{
    public interface ICustomerService
    {
        Task<ResponseWrapper<CreateCustomerRes>> CreateCustomer(CreateCustomerReq request, string createdBy);
        // === MODIFIED: Return type changed to CustomerRes to match implementation ===
        Task<ResponseWrapper<PagedResult<CustomerRes>>> GetCustomerByFilter(CustomerFilter filter);
        // ===========================================================================
        Task<ResponseWrapper<Customer>> GetCustomerById(Guid customerId);
        Task<ResponseWrapper<Customer>> DeleteCustomer(Guid customerId);

        // === NEW METHOD SIGNATURE ===
        /// <summary>
        /// Retrieves a paginated list of customer summaries, including their full name and all associated loan IDs.
        /// </summary>
        /// <param name="pageNumber">The current page number (1-based).</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <returns>A ResponseWrapper containing a PagedResult of CustomerLoanSummaryRes objects.</returns>
        Task<ResponseWrapper<PagedResult<CustomerLoanSummaryRes>>> GetCustomerLoanSummaries(int pageNumber, int pageSize);
        // ============================
    }
}
