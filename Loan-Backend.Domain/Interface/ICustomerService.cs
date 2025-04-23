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
        Task<ResponseWrapper<PagedResult<Customer>>> GetCustomerByFilter(CustomerFilter filter);
        Task<ResponseWrapper<Customer>> GetCustomerById(Guid customerId);
        Task<ResponseWrapper<Customer>> DeleteCustomer(Guid customerId);
    }
}
