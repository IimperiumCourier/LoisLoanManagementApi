using Loan_Backend.Domain.Interface;
using Loan_Backend.Infrastructure.Repository;
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
    public class QuarterlyProfitAnalyticsStrategy : IProfitAnalyticsStrategy
    {
        private readonly IUnitOfWork _unitOfWork;
        public QuarterlyProfitAnalyticsStrategy(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseWrapper<ProfitAnalyticsResult>> GetProfitAnalysis(ProfitAnalyticsRequest request)
        {
            if (request.Year == null || request.Quarter == null)
                return ResponseWrapper<ProfitAnalyticsResult>.Error("Year and Quarter are required for quarterly analytics.");

            var year = request.Year.Value;
            var quarter = request.Quarter.Value;

            var monthStart = (quarter - 1) * 3 + 1;
            var from = new DateTime(year, monthStart, 1);
            var to = from.AddMonths(3).AddDays(-1);

            var loans = await _unitOfWork.CustomerLoanRepository
                .FindAsync(l => l.CreatedDate >= from && l.CreatedDate <= to);

            var disbursed = loans.Where(l => l.Status != LoanStatusEnum.Declined.ToString());
            var repaid = loans.Where(l => l.Status == LoanStatusEnum.Paid.ToString());
            var defaulted = loans.Where(l => l.Status == LoanStatusEnum.Defaulted.ToString());

            var result = new ProfitAnalyticsResult
            {
                Period = $"Q{quarter} {year}",
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
