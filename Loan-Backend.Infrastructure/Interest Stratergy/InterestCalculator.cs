using Loan_Backend.Domain.Interface;
using Loan_Backend.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loan_Backend.Infrastructure.Interest_Stratergy
{
    public class InterestCalculator
    {
        private readonly IInterestStrategy _strategy;

        public InterestCalculator(string loanGroup)
        {
            var frequency = (InterestFrequencyEnum)Enum.Parse(typeof(InterestFrequencyEnum), loanGroup);

            _strategy = frequency switch
            {
                InterestFrequencyEnum.Weekly => new WeeklyInterestStrategy(),
                InterestFrequencyEnum.Monthly => new MonthlyInterestStrategy(),
                _ => throw new NotImplementedException("Strategy not implemented.")
            };
        }

        public decimal Calculate(decimal principal, decimal annualRate, int durationInWeeks)
        {
            return _strategy.CalculateInterest(principal, annualRate, durationInWeeks);
        }
    }

}
