using Loan_Backend.Domain.Interface;
using Loan_Backend.SharedKernel;
using Loan_Backend.SharedKernel.Model.Request;
using Loan_Backend.SharedKernel.Model.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loan_Backend.Infrastructure.Service
{
    public class CustomProfitAnalyticsStrategy : IProfitAnalyticsStrategy
    {
        private readonly IUnitOfWork _unitOfWork;
        public CustomProfitAnalyticsStrategy(IUnitOfWork unitOfWork) 
        { 
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseWrapper<ProfitAnalyticsResult>> GetProfitAnalysis(ProfitAnalyticsRequest request)
        {
            ProfitAnalyticsResult profitAnalyticsResult  =  new ProfitAnalyticsResult();

            if (request.From == null || request.To == null)
            {
                return ResponseWrapper<ProfitAnalyticsResult>.Error("From and To dates are required for Custom analytics.");
            }

            var interestEarned = await _unitOfWork.CustomerLoanRepository.SumAsync(loan => loan.InterestEarned);
            var totalDisbursement = await _unitOfWork.CustomerLoanRepository.SumAsync(loan => loan.Amount, 
                                                                                      loan => loan.Status != LoanStatusEnum.Declined.ToString());
            var totalRepayment = await _unitOfWork.CustomerLoanRepository.SumAsync(loan => loan.RepaymentAmount, loan => loan.Status == LoanStatusEnum.Paid.ToString());
            var totalOverDueAmount = await _unitOfWork.CustomerLoanRepository.SumAsync(loan => loan.RepaymentAmount,
                                                                                        loan => loan.Status == LoanStatusEnum.Defaulted.ToString());
            profitAnalyticsResult.TotalRepayment = totalRepayment;
            profitAnalyticsResult.Profit = interestEarned;
            profitAnalyticsResult.TotalDisbursement = totalDisbursement;
            profitAnalyticsResult.OverdueLoans = totalOverDueAmount;
            profitAnalyticsResult.TotalOverdueAmount = totalOverDueAmount;
            profitAnalyticsResult.Period = $"{request.From.Value.FormatDateWithOrdinal()} to {request.To.Value.FormatDateWithOrdinal()}";

            return ResponseWrapper<ProfitAnalyticsResult>.Success(profitAnalyticsResult);
        }
    }
}
