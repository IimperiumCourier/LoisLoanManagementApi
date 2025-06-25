using Loan_Backend.Domain.Entities;
using Loan_Backend.Domain.Interface;
using Loan_Backend.SharedKernel;
using Loan_Backend.SharedKernel.Model.DTO;
using Loan_Backend.SharedKernel.Model.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loan_Backend.Infrastructure.Service
{
    public class PaymentLogService : IPaymentLogService
    {
        private readonly IUnitOfWork unitOfWork;
        public PaymentLogService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<ResponseWrapper<string>> ApprovePayment(Guid paymentLogId, string approver)
        {
            var paymentLog = await unitOfWork.PaymentLogRepository.GetByIdAsync(paymentLogId);
            if (paymentLog is null)
                return ResponseWrapper<string>.Error("Payment log not found.");

            var loan = await unitOfWork.CustomerLoanRepository.GetByIdAsync(paymentLog.LoanId);
            if (loan is null)
                return ResponseWrapper<string>.Error("Loan record not found.");

            paymentLog.Approve(approver);

            var approvedLogs = await unitOfWork.PaymentLogRepository.FindAsync(e =>
                e.LoanId == loan.Id &&
                e.Status == PaymentStatusEnum.Approved.ToString());

            var allPaymentLogs = approvedLogs?.ToList() ?? new List<PaymentLog>();

            allPaymentLogs.Add(paymentLog);

            var isLoanPaymentComplete = loan.IsPaymentCompleted(allPaymentLogs);
            if (isLoanPaymentComplete)
            {
                await unitOfWork.CustomerLoanRepository.UpdateAsync(loan);
            }

            var repaymentPlan = await unitOfWork.RepaymentPlanRepository.GetByIdAsync(paymentLog.RepaymentId);
            if (repaymentPlan is null)
            {
                return ResponseWrapper<string>.Error("Repayment plan record not found.");
            }

            repaymentPlan.MarkRepaymentAsPaid();

            await unitOfWork.RepaymentPlanRepository.UpdateAsync(repaymentPlan);

            await unitOfWork.PaymentLogRepository.UpdateAsync(paymentLog);

            var dbCount = await unitOfWork.SaveAsync();
            if(dbCount <= 0)
            {
                return ResponseWrapper<string>.Error("Operation failed.");
            }

            return ResponseWrapper<string>.Success("Payment approved successfully.");
        }


        public async Task<ResponseWrapper<string>> DeclinePayment(Guid paymentLogId, string approver)
        {
            var paymentLog = await unitOfWork.PaymentLogRepository.GetByIdAsync(paymentLogId);
            if (paymentLog is null)
                return ResponseWrapper<string>.Error("Payment log not found.");

            paymentLog.Decline(approver);

            await unitOfWork.PaymentLogRepository.UpdateAsync(paymentLog);

            var dbCount = await unitOfWork.SaveAsync();
            if (dbCount <= 0)
            {
                return ResponseWrapper<string>.Error("Operation failed.");
            }

            return ResponseWrapper<string>.Success("Payment declined successfully.");
        }

        public async Task<ResponseWrapper<PagedResult<PaymentLogDto>>> GetPaymentLogs(PaymentStatusEnum? status, int pageNum = 1, int pageSize = 10)
        {
            var paymentLogDtos = new List<PaymentLogDto>();
            var loanPaymentLogs = await unitOfWork.PaymentLogRepository.GetAllAsync();
            if (loanPaymentLogs == null)
            {
                return ResponseWrapper<PagedResult<PaymentLogDto>>.Error("No record found.");
            }

            if(status != null)
            {
                string statusStr = status.ToString()!;
                loanPaymentLogs = loanPaymentLogs.Where(e => e.Status == statusStr);
            }

            int totalCount = loanPaymentLogs.Count();

            var items = loanPaymentLogs
                .Skip((pageNum - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            foreach (var item in items) 
            {
                var loan = await unitOfWork.CustomerLoanRepository.GetByIdAsync(item.LoanId);

                var customerInfo = await unitOfWork.CustomerRepository.GetByIdAsync(loan.CustomerId);

                paymentLogDtos.Add(new PaymentLogDto
                {
                    CustomerId = customerInfo!.Id.ToString(),
                    AmountPaid = item.AmountPaid,
                    Approved = item.Approved,
                    ApprovedBy = item.ApprovedBy,
                    CurrencyCode = item.CurrencyCode,
                    CustomerEmail = customerInfo!.Email,
                    CustomerName = customerInfo!.FullName,
                    CustomerPhone = customerInfo!.Phonenumber,
                    DateLogged = item.DateLogged,
                    LoanId = item.LoanId,
                    LoggedBy = item.LoggedBy,
                    PaymentLogId = item.Id,
                    Status = item.Status

                });
            }

            var pagedResult = new PagedResult<PaymentLogDto>
            {
                Items = paymentLogDtos,
                TotalCount = totalCount,
                PageNumber = pageNum,
                PageSize = pageSize
            };

            return ResponseWrapper<PagedResult<PaymentLogDto>>.Success(pagedResult);
        }

        public async Task<ResponseWrapper<PagedResult<PaymentLogDto>>> GetPaymentLogUsingLoanId(Guid loanId, PaymentStatusEnum? status, int pageNum = 1, int pageSize = 10)
        {
            var loanPaymentLogs = await unitOfWork.PaymentLogRepository.FindAsync(e => e.LoanId == loanId);
            if (loanPaymentLogs == null)
            {
                return ResponseWrapper<PagedResult<PaymentLogDto>>.Error("No record found.");
            }

            var loan = await unitOfWork.CustomerLoanRepository.GetByIdAsync(loanId);

            var customerInfo = await unitOfWork.CustomerRepository.GetByIdAsync(loan.CustomerId);

            if (status != null)
            {
                string statusStr = status.ToString()!;
                loanPaymentLogs = loanPaymentLogs.Where(e => e.Status == statusStr);
            }

            int totalCount = loanPaymentLogs.Count();

            var items = loanPaymentLogs
                .Skip((pageNum - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var pagedResult = new PagedResult<PaymentLogDto>
            {
                Items = items.Select(e => new PaymentLogDto
                {
                    CustomerId = customerInfo!.Id.ToString(),
                    AmountPaid = e.AmountPaid,
                    Approved = e.Approved,
                    ApprovedBy = e.ApprovedBy,
                    CurrencyCode = e.CurrencyCode,
                    CustomerEmail = customerInfo!.Email,
                    CustomerName = customerInfo!.FullName,
                    CustomerPhone = customerInfo!.Phonenumber,
                    DateLogged = e.DateLogged,
                    LoanId = loanId,
                    LoggedBy = e.LoggedBy,
                    PaymentLogId = e.Id,
                    Status = e.Status

                }),
                TotalCount = totalCount,
                PageNumber = pageNum,
                PageSize = pageSize
            };

            return ResponseWrapper<PagedResult<PaymentLogDto>>.Success(pagedResult);
        }

        public async Task<ResponseWrapper<PaymentLog>> LogPayment(CreatePaymentLogReq logRequest, string loggedBy)
        {
            if (string.IsNullOrWhiteSpace(loggedBy))
            {
                return ResponseWrapper<PaymentLog>.Error("Request is unauthorized.");
            }

            var loan = await unitOfWork.CustomerLoanRepository.GetByIdAsync(logRequest.LoanId);
            if (loan is null)
            {
                return ResponseWrapper<PaymentLog>.Error("Loan record not found.");
            }

            var repaymentPlan = await unitOfWork.RepaymentPlanRepository.GetByIdAsync(logRequest.RepaymentId);
            if (repaymentPlan is null) { 
                return ResponseWrapper<PaymentLog>.Error("Repayment plan record not found.");
            }

            if (repaymentPlan.LoanId != loan.Id)
            {
                return ResponseWrapper<PaymentLog>.Error($"Repayment plan with id ::: {logRequest.RepaymentId} does not belong to loan with id ::: {logRequest.LoanId}.");
            }

            if (!string.Equals(loan.CurrencyCode, logRequest.CurrencyCode, StringComparison.OrdinalIgnoreCase))
            {
                return ResponseWrapper<PaymentLog>.Error("Currency code does not match loan record.");
            }

            var paymentLog = PaymentLog.Create(logRequest.LoanId, logRequest.Amount, logRequest.CurrencyCode, loggedBy, logRequest.RepaymentId);
            await unitOfWork.PaymentLogRepository.AddAsync(paymentLog);

            var count = await unitOfWork.SaveAsync();
            if(count <= 0)
            {
                return ResponseWrapper<PaymentLog>.Error("Operation failed.");
            }

            return ResponseWrapper<PaymentLog>.Success(paymentLog);
        }
    }
}
