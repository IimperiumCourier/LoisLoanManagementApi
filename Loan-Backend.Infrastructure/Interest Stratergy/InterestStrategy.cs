using Loan_Backend.Domain.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loan_Backend.Infrastructure.Interest_Stratergy
{
    public class InterestStrategy : IInterestStrategy
    {
        private const decimal interestRate = 0.26M;
        public decimal CalculateInterest(decimal principal, decimal annualRate, int durationInWeeks)
        {
            decimal interest = principal * interestRate;
            return interest;
        }
    }

}
