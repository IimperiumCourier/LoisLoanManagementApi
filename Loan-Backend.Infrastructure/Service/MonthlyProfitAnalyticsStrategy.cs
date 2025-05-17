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
    public class MonthlyProfitAnalyticsStrategy : IProfitAnalyticsStrategy
    {
        private readonly IUnitOfWork _unitOfWork;
        public MonthlyProfitAnalyticsStrategy(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseWrapper<ProfitAnalyticsResult>> GetProfitAnalysis(ProfitAnalyticsRequest request)
        {
            if (request.Year == null || request.Month == null)
                return ResponseWrapper<ProfitAnalyticsResult>.Error("Year and Month are required for monthly analytics.");

            var year = request.Year.Value;
            var month = request.Month.Value;

            if (month < 1 || month > 12)
            { 
                return ResponseWrapper<ProfitAnalyticsResult>.Error("Month must be between 1 and 12.");
            }

            var loans = await _unitOfWork.CustomerLoanRepository
                .FindAsync(l => l.CreatedDate.Year == year && l.CreatedDate.Month == month);

            var disbursed = loans.Where(l => l.Status != LoanStatusEnum.Declined.ToString());
            var repaid = loans.Where(l => l.Status == LoanStatusEnum.Paid.ToString());
            var defaulted = loans.Where(l => l.Status == LoanStatusEnum.Defaulted.ToString());

            var result = new ProfitAnalyticsResult
            {
                Period = $"{new DateTime(year, month, 1):MMM yyyy}",
                TotalDisbursement = disbursed.Sum(l => l.Amount),
                TotalRepayment = repaid.Sum(l => l.RepaymentAmount),
                InterestEarned = loans.Sum(l => l.InterestEarned),
                OverdueLoans = defaulted.Count(),
                TotalOverdueAmount = defaulted.Sum(l => l.RepaymentAmount),
                Profit = loans.Sum(l => l.InterestEarned)
            };

            return ResponseWrapper<ProfitAnalyticsResult>.Success(result);
        }
    }
}
