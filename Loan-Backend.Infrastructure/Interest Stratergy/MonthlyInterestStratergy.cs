using Loan_Backend.Domain.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loan_Backend.Infrastructure.Interest_Stratergy
{
    public class MonthlyInterestStrategy : IInterestStrategy
    {
        public decimal CalculateInterest(decimal principal, decimal annualRate, int durationInWeeks)
        {
            double months = durationInWeeks / 4.345; // Approx. weeks per month
            double timeInYears = months / 12.0;
            decimal interest = (principal * annualRate * (decimal)timeInYears) / 100;
            return interest;
        }
    }

}
