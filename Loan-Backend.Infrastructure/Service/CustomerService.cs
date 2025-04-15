using Loan_Backend.Domain.Entities;
using Loan_Backend.Domain.Interface;
using Loan_Backend.Infrastructure.Filter;
using Loan_Backend.Infrastructure.Interest_Stratergy;
using Loan_Backend.SharedKernel;
using Loan_Backend.SharedKernel.Model.DTO;
using Loan_Backend.SharedKernel.Model.Request;
using Loan_Backend.SharedKernel.Model.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Loan_Backend.Infrastructure.Service
{
    public class CustomerService : ICustomerService
    {
        private IUnitOfWork unitOfWork;

        public CustomerService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;        
        }

        public async Task<ResponseWrapper<CreateCustomerRes>> CreateCustomer(CreateCustomerReq request, string createdBy)
        {

            if (request == null)
            {
                return ResponseWrapper<CreateCustomerRes>.Error("Request is invalid.");
            }

            if (request.LoanPreference == null)
            {
                return ResponseWrapper<CreateCustomerRes>.Error("Pass a valid loan preference.");
            }

            var existingCustomer = await unitOfWork.CustomerRepository.FindAsync(e => e.Email == request.Email || e.Phonenumber == request.Phonenumber);
            if (existingCustomer.Any())
            {
                return ResponseWrapper<CreateCustomerRes>.Error("Customer has an existing record.");
            }

            var customer = Customer.Create(request);
            var customerLoan = customer.CreateLoan(request.LoanPreference, createdBy);

            var interestCalculator = new InterestCalculator(request.LoanPreference.LoanGroup);
            if(interestCalculator == null)
            {
                return ResponseWrapper<CreateCustomerRes>.Error("Repayment amount calculation failed.");
            }

            var repaymentAmt = customerLoan.Amount + interestCalculator.Calculate(customerLoan.Amount,
                                                                                  customerLoan.InterestRatePercent,
                                                                                  customerLoan.DurationInWeeks);
            customerLoan.AddRepaymentAmount(repaymentAmt);
            await unitOfWork.CustomerLoanRepository.AddAsync(customerLoan);
            await unitOfWork.CustomerRepository.AddAsync(customer);

            var dbResponse = await unitOfWork.SaveAsync();
            if(dbResponse <= 0)
            {
                return ResponseWrapper<CreateCustomerRes>.Error("Operation failed.");
            }

            return ResponseWrapper<CreateCustomerRes>.Success(CreateCustomerRes.Create(customer.Id, customerLoan.Id),"Operation was successful");
        }

        public async Task<ResponseWrapper<PagedResult<Customer>>> GetCustomerByFilter(CustomerFilter filter)
        {
            var expression = CustomerFilterBuilder.Build(filter);
            var queryResult = await unitOfWork.CustomerRepository.FindAsync(expression);
            if(queryResult == null)
            {
                return ResponseWrapper<PagedResult<Customer>>.Error("No record found.");
            }

            int totalCount = queryResult.Count();

            var items = queryResult
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToList();

            var pagedResult = new PagedResult<Customer>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            };

            return ResponseWrapper<PagedResult<Customer>>.Success(pagedResult);
        }

        public async Task<ResponseWrapper<Customer>> GetCustomerById(Guid customerId)
        {
            var queryResult = await unitOfWork.CustomerRepository.FindAsync(customer => customer.Id == customerId);
            if (queryResult == null)
            {
                return ResponseWrapper<Customer>.Error("No record found.");
            }

            var customer = queryResult.FirstOrDefault();
            if(customer == null)
            {
                return ResponseWrapper<Customer>.Error("No record found.");
            }

            return ResponseWrapper<Customer>.Success(customer);
        }
    }
}
