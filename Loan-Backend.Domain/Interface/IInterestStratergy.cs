using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loan_Backend.Domain.Interface
{
    public interface IInterestStrategy
    {
        decimal CalculateInterest(decimal principal, decimal annualRate, int durationInWeeks);
    }

}
