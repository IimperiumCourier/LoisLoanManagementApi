using Loan_Backend.Domain.Interface;
using Loan_Backend.SharedKernel;
using System;

namespace Loan_Backend.Infrastructure.Service
{
    public class ProfitAnalyticsStrategyFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public ProfitAnalyticsStrategyFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public IProfitAnalyticsStrategy CreateStrategy(ProfitAnalyticsEnum strategyType)
        {
            return strategyType switch
            {
                ProfitAnalyticsEnum.Monthly => ResolveStrategy<MonthlyProfitAnalyticsStrategy>(),
                ProfitAnalyticsEnum.Quarterly => ResolveStrategy<QuarterlyProfitAnalyticsStrategy>(),
                ProfitAnalyticsEnum.Yearly => ResolveStrategy<YearlyProfitAnalyticsStrategy>(),
                ProfitAnalyticsEnum.Custom => ResolveStrategy<CustomProfitAnalyticsStrategy>(),
                _ => throw new NotSupportedException($"Profit analytics strategy '{strategyType}' is not supported.")
            };
        }

        private IProfitAnalyticsStrategy ResolveStrategy<T>() where T : class, IProfitAnalyticsStrategy
        {
            var strategy = _serviceProvider.GetService(typeof(T)) as IProfitAnalyticsStrategy;

            if (strategy == null)
                throw new InvalidOperationException($"Failed to resolve strategy of type {typeof(T).Name}");

            return strategy;
        }
    }
}
