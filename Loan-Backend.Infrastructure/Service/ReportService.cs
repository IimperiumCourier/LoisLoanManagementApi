using Loan_Backend.Domain.Interface;
using Loan_Backend.SharedKernel;
using Loan_Backend.SharedKernel.Model.Request;
using Loan_Backend.SharedKernel.Model.Response;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loan_Backend.Infrastructure.Service
{
    public class ReportService : IReportService
    {
        private readonly ProfitAnalyticsStrategyFactory _strategyFactory;

        public ReportService(IOptions<ProfitAnalyticsStrategyFactory> options)
        {
             _strategyFactory = options.Value;
        }

        public async Task<ResponseWrapper<ProfitAnalyticsResult>> AnalyzeProfit(ProfitAnalyticsRequest request)
        {
            var profitAnalyzer = _strategyFactory.CreateStrategy(request.Filter);

            return await profitAnalyzer.GetProfitAnalysis(request);
        }
    }
}
