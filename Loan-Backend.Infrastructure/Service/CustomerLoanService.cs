using Loan_Backend.Domain.Entities;
using Loan_Backend.Domain.Interface;
using Loan_Backend.Infrastructure.Interest_Stratergy;
using Loan_Backend.SharedKernel;
using Loan_Backend.SharedKernel.Model.DTO;
using Loan_Backend.SharedKernel.Model.Request;
using Loan_Backend.SharedKernel.Model.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loan_Backend.Infrastructure.Service
{
    public class CustomerLoanService(IUnitOfWork unitOfWork, IRepaymentPlanCalculator repaymentPlanCalculator) : ICustomerLoanService
    {

        public async Task<ResponseWrapper<List<LoanRepaymentPlanResponse>>> GetCustomerLoanRepaymentPlan(Guid loanId)
        {
            var customerLoan = await unitOfWork.CustomerLoanRepository.GetByIdAsync(loanId);
            if (customerLoan == null)
            {
                return ResponseWrapper<List<LoanRepaymentPlanResponse>>.Error("No record found.");
            }

            var repaymentPlan = await unitOfWork.RepaymentPlanRepository.FindAsync(e => e.LoanId == loanId);
            var customer = await unitOfWork.CustomerRepository.GetByIdAsync(customerLoan.CustomerId);
            if(customer == null)
            {
                return ResponseWrapper<List<LoanRepaymentPlanResponse>>.Error("customer record not found.");
            }

            var result = repaymentPlan.Select(e => new LoanRepaymentPlanResponse
            {
                AmountBorrowed = customerLoan.Amount,
                AmountPerInstallment = customerLoan.AmountPerInstallment,
                AmountToBeRepaid = customerLoan.RepaymentAmount,
                DueDate = e.DueDate,
                Fullname = customer.FullName,
                IsPaid = e.IsPaid
            }).ToList();

            return ResponseWrapper<List<LoanRepaymentPlanResponse>>.Success(result);
        }

        public async Task<ResponseWrapper<List<LoanRepaymentPlanResponse>>> GetDueLoanRepaymentPlan(DateTime from, DateTime to)
        {
            var result = new List<LoanRepaymentPlanResponse>();
            var dueRepaymentPlans = await unitOfWork.RepaymentPlanRepository.FindAsync(e => !e.IsPaid && (e.DueDate >= from && e.DueDate <= to));

            foreach(var repaymentPlan in dueRepaymentPlans)
            {
                var loan = await unitOfWork.CustomerLoanRepository.GetByIdAsync(repaymentPlan.LoanId);
                if(loan == null)
                {
                    continue;
                }

                var customer = await unitOfWork.CustomerRepository.GetByIdAsync(loan.CustomerId);
                if(customer == null) {  
                    continue;
                }

                result.Add(new LoanRepaymentPlanResponse
                {
                    AmountBorrowed = loan.Amount,
                    AmountPerInstallment = loan.AmountPerInstallment,
                    AmountToBeRepaid = loan.RepaymentAmount,
                    DueDate = repaymentPlan.DueDate,
                    Fullname = customer.FullName,
                    IsPaid = repaymentPlan.IsPaid
                });
            }


            return ResponseWrapper<List<LoanRepaymentPlanResponse>>.Success(result);
        }

        public async Task<ResponseWrapper<string>> ApproveLoan(Guid customerLoanId, string approver)
        {
            var customerLoan = await unitOfWork.CustomerLoanRepository.GetByIdAsync(customerLoanId); 
            if(customerLoan == null)
            {
                return ResponseWrapper<string>.Error("No record found.");
            }

            customerLoan.AddApproverInfo(approver);

            var computedRepaymentPlan = repaymentPlanCalculator.ComputeRepaymentPlan(customerLoan);

            customerLoan.SetInstallmentDetails(computedRepaymentPlan.AmountPerInstallment, computedRepaymentPlan.NumberOfPaymentInstallments);

            var customerRepaymentPlans = customerLoan.GetRepaymentPlan(computedRepaymentPlan.DueDates);

            await unitOfWork.CustomerLoanRepository.UpdateAsync(customerLoan);

            await unitOfWork.RepaymentPlanRepository.AddRangeAsync(customerRepaymentPlans);

            var count = await unitOfWork.SaveAsync();
            if(count <= 0)
            {
                return ResponseWrapper<string>.Error("Operation failed.");
            }

            return ResponseWrapper<string>.Success("Operation completed successfully.");
        }

        public async Task<ResponseWrapper<CustomerLoan>> CreateLoan(CreateLoanReq request, string createdBy)
        {
            if (request.LoanPreference == null)
            {
                return ResponseWrapper<CustomerLoan>.Error("Pass a valid loan preference.");
            }

            var customer = await unitOfWork.CustomerRepository.GetByIdAsync(request.CustomerId);
            if(customer == null)
            {
                return ResponseWrapper<CustomerLoan>.Error("Customer record not found.");
            }

            var customerLoan = customer.CreateLoan(request.LoanPreference, createdBy);

            var interestCalculator = new InterestCalculator(request.LoanPreference.LoanGroup);
            if (interestCalculator == null)
            {
                return ResponseWrapper<CustomerLoan>.Error("Repayment amount calculation failed.");
            }

            var repaymentAmt = customerLoan.Amount + interestCalculator.Calculate(customerLoan.Amount,
                                                                                  customerLoan.InterestRatePercent,
                                                                                  customerLoan.DurationInWeeks);
            customerLoan.AddRepaymentAmount(repaymentAmt);
            await unitOfWork.CustomerLoanRepository.AddAsync(customerLoan);

            var dbResponse = await unitOfWork.SaveAsync();
            if (dbResponse <= 0)
            {
                return ResponseWrapper<CustomerLoan>.Error("Operation failed.");
            }

            return ResponseWrapper<CustomerLoan>.Success(customerLoan);
        }

        public async Task<ResponseWrapper<string>> DeclineLoan(Guid customerLoanId)
        {
            var customerLoan = await unitOfWork.CustomerLoanRepository.GetByIdAsync(customerLoanId);
            if (customerLoan == null)
            {
                return ResponseWrapper<string>.Error("No record found.");
            }

            customerLoan.Declined();
            await unitOfWork.CustomerLoanRepository.AddAsync(customerLoan);

            var dbResponse = await unitOfWork.SaveAsync();
            if (dbResponse <= 0)
            {
                return ResponseWrapper<string>.Error("Operation failed.");
            }

            return ResponseWrapper<string>.Success("Operation succeded.");
        }

        public async Task<ResponseWrapper<string>> DefaultLoan(Guid customerLoanId)
        {
            var customerLoan = await unitOfWork.CustomerLoanRepository.GetByIdAsync(customerLoanId);
            if (customerLoan == null)
            {
                return ResponseWrapper<string>.Error("No record found.");
            }

            customerLoan.Defaulted();
            await unitOfWork.CustomerLoanRepository.AddAsync(customerLoan);

            var dbResponse = await unitOfWork.SaveAsync();
            if (dbResponse <= 0)
            {
                return ResponseWrapper<string>.Error("Operation failed.");
            }

            return ResponseWrapper<string>.Success("Operation succeded.");
        }

        public async Task<ResponseWrapper<PagedResult<CustomerLoan>>> GetLoanByCustomerId(Guid customerId, LoanStatusEnum? status,
                                                                                InterestFrequencyEnum? type, int pageNum = 1, int pageSize = 10)
        {
            var customerLoans = await unitOfWork.CustomerLoanRepository.FindAsync(e => e.CustomerId == customerId);
            if(customerLoans == null)
            {
                return ResponseWrapper<PagedResult<CustomerLoan>>.Error("No record found.");
            }

            if (type != null)
            {
                string typeStr = type.ToString()!;
                customerLoans = customerLoans.Where(e => e.LoanGroup == typeStr);
            }

            if (status != null)
            {
                string statusStr = status.ToString()!;
                customerLoans = customerLoans.Where(e => e.Status == statusStr);
            }

            int totalCount = customerLoans.Count();

            var items = customerLoans
                .Skip((pageNum - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var pagedResult = new PagedResult<CustomerLoan>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNum,
                PageSize = pageSize
            };

            return ResponseWrapper<PagedResult<CustomerLoan>>.Success(pagedResult);
        }

        public async Task<ResponseWrapper<CustomerLoan>> GetLoanById(Guid customerLoanId)
        {
            var loan = await unitOfWork.CustomerLoanRepository.GetByIdAsync(customerLoanId);
            if(loan == null)
            {
                return ResponseWrapper<CustomerLoan>.Error("No record found.");
            }

            return ResponseWrapper<CustomerLoan>.Success(loan);
        }

        public async Task<ResponseWrapper<PagedResult<CustomerLoan>>> GetLoans(
                                                                                LoanStatusEnum? status,
                                                                                InterestFrequencyEnum? type,
                                                                                int pageNum = 1, int pageSize = 10)
        {
            var customerLoans = await unitOfWork.CustomerLoanRepository.GetAllAsync();
            if (!customerLoans.Any())
            {
                return ResponseWrapper<PagedResult<CustomerLoan>>.Error("No record found.");
            }

            if (type != null)
            {
                string typeStr = type.ToString()!;
                customerLoans = customerLoans.Where(e => e.LoanGroup == typeStr);
            }

            if (status != null)
            {
                string statusStr = status.ToString()!;
                customerLoans = customerLoans.Where(e => e.Status == statusStr);
            }

            

            int totalCount = customerLoans.Count();

            var items = customerLoans
                .Skip((pageNum - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var pagedResult = new PagedResult<CustomerLoan>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNum,
                PageSize = pageSize
            };

            return ResponseWrapper<PagedResult<CustomerLoan>>.Success(pagedResult);
        }

    }
}
