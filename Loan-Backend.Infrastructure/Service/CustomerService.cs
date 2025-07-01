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
using System.Linq; // Required for .Select(), .ToList(), .Any(), .Count(), .Skip(), .Take()
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
// using static Microsoft.EntityFrameworkCore.DbLoggerCategory; // Removed if not used, can cause issues if not configured

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
            if (interestCalculator == null)
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
            if (dbResponse <= 0)
            {
                return ResponseWrapper<CreateCustomerRes>.Error("Operation failed.");
            }

            return ResponseWrapper<CreateCustomerRes>.Success(CreateCustomerRes.Create(customer.Id, customerLoan.Id), "Operation was successful");
        }

        public async Task<ResponseWrapper<Customer>> DeleteCustomer(Guid customerId)
        {
            var customer = await unitOfWork.CustomerRepository.GetByIdAsync(customerId);
            if (customer == null)
            {
                return ResponseWrapper<Customer>.Error("No record found.");
            }

            customer.Deactivate();

            await unitOfWork.CustomerRepository.UpdateAsync(customer);
            var count = await unitOfWork.SaveAsync();

            if (count <= 0)
            {
                return ResponseWrapper<Customer>.Error("Customer record was not successfully deleted.");
            }

            return ResponseWrapper<Customer>.Error("Customer record was successfully deleted.");
        }

        // === MODIFIED METHOD: GetCustomerByFilter to return CustomerRes DTO with LoanIds ===
        // This method now correctly implements the ICustomerService signature for CustomerRes
        public async Task<ResponseWrapper<PagedResult<CustomerRes>>> GetCustomerByFilter(CustomerFilter filter)
        {
            var expression = CustomerFilterBuilder.Build(filter);
            var queryResult = await unitOfWork.CustomerRepository.FindAsync(expression);
            if (queryResult == null || !queryResult.Any()) // Check if queryResult is null or empty
            {
                return ResponseWrapper<PagedResult<CustomerRes>>.Error("No record found.");
            }

            int totalCount = queryResult.Count();

            // Paginate the raw Customer entities first
            var pagedCustomers = queryResult
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToList();

            var customerResponses = new List<CustomerRes>();

            foreach (var customer in pagedCustomers)
            {
                // Fetch all loan IDs for the current customer
                // Corrected: Await FindAsync before calling Select and ToList()
                var customerLoanIds = (await unitOfWork.CustomerLoanRepository
                    .FindAsync(l => l.CustomerId == customer.Id))
                    .Select(l => l.Id)
                    .ToList();

                // Map Customer entity to CustomerRes DTO
                var customerRes = new CustomerRes
                {
                    Id = customer.Id,
                    FullName = customer.FullName,
                    Phonenumber = customer.Phonenumber,
                    Email = customer.Email,
                    DateOfBirth = customer.DateOfBirth,
                    Gender = customer.Gender,
                    MaritalStatus = customer.MaritalStatus,
                    ResidentialAddress = customer.ResidentialAddress,
                    EmploymentStatus = customer.EmploymentStatus,
                    MonthlyIncome = customer.MonthlyIncome,
                    SelfieUrl = customer.SelfieUrl,
                    // Map nested objects (Identification, Guarantor)
                    // Assuming Customer.Identification is a complex object/value object in the entity
                    Identification = customer.Identification != null ? new Identification
                    {
                        IdentificationType = customer.Identification.IdentificationType,
                        IdentificationNumber = customer.Identification.IdentificationNumber,
                        IdentificationUrl = customer.Identification.IdentificationUrl
                    } : null,
                    // === FIX: Removed mapping for Guarantor.Identification as the entity does not have this property ===
                    // This assumes Loan_Backend.Domain.Entities.Guarantor does NOT have a public 'Identification' property
                    // or flattened identification properties (IdentificationType, etc.)
                    Guarantor = customer.Guarantor != null ? new Guarantor
                    {
                        FullName = customer.Guarantor.FullName,
                        RelationshipToCustomer = customer.Guarantor.RelationshipToCustomer,
                        PhoneNumber = customer.Guarantor.PhoneNumber,
                        EmailAddress = customer.Guarantor.EmailAddress,
                        ResidentialAddress = customer.Guarantor.ResidentialAddress,
                        // Identification is set to null here because the entity's Guarantor does not have it.
                        // If your entity's Guarantor *does* have identification properties (nested or flattened),
                        // you will need to adjust this mapping accordingly.
                        Identification = null // Set to null to avoid compilation error
                    } : null,
                    // Assign the list of loan IDs
                    LoanIds = customerLoanIds // This is the key change!
                };
                customerResponses.Add(customerRes);
            }

            var pagedResult = new PagedResult<CustomerRes>
            {
                Items = customerResponses,
                TotalCount = totalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            };

            return ResponseWrapper<PagedResult<CustomerRes>>.Success(pagedResult);
        }
        // === END OF MODIFIED METHOD ===

        public async Task<ResponseWrapper<Customer>> GetCustomerById(Guid customerId)
        {
            var customer = await unitOfWork.CustomerRepository.GetByIdAsync(customerId);
            if (customer == null)
            {
                return ResponseWrapper<Customer>.Error("No record found.");
            }

            return ResponseWrapper<Customer>.Success(customer);
        }

        // === NEW METHOD: GetCustomerLoanSummaries ===
        /// <summary>
        /// Retrieves a paginated list of customer summaries, including their full name and all associated loan IDs.
        /// </summary>
        /// <param name="pageNumber">The current page number (1-based).</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <returns>A ResponseWrapper containing a PagedResult of CustomerLoanSummaryRes objects.</returns>
        public async Task<ResponseWrapper<PagedResult<CustomerLoanSummaryRes>>> GetCustomerLoanSummaries(int pageNumber, int pageSize)
        {
            // Fetch all customers (or a filtered set if needed)
            var allCustomers = await unitOfWork.CustomerRepository.GetAllAsync();

            if (allCustomers == null || !allCustomers.Any())
            {
                return ResponseWrapper<PagedResult<CustomerLoanSummaryRes>>.Error("No customer records found.");
            }

            int totalCount = allCustomers.Count();

            // Apply pagination
            var pagedCustomers = allCustomers
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var customerSummaries = new List<CustomerLoanSummaryRes>();

            foreach (var customer in pagedCustomers)
            {
                // Fetch all loan IDs for the current customer
                var customerLoanIds = (await unitOfWork.CustomerLoanRepository
                    .FindAsync(l => l.CustomerId == customer.Id))
                    .Select(l => l.Id)
                    .ToList();

                customerSummaries.Add(new CustomerLoanSummaryRes
                {
                    CustomerId = customer.Id,
                    FullName = customer.FullName,
                    LoanIds = customerLoanIds
                });
            }

            var pagedResult = new PagedResult<CustomerLoanSummaryRes>
            {
                Items = customerSummaries,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            return ResponseWrapper<PagedResult<CustomerLoanSummaryRes>>.Success(pagedResult);
        }
        // === END NEW METHOD ===

        // --- Keep other methods as they are ---
        // ... (e.g., GetLoans, GetLoanById, ApproveLoan, DeclineLoan, DefaultLoan from CustomerLoanService if they were here) ...
    }
}
