using Loan_Backend.Domain.Entities;
using Loan_Backend.SharedKernel.Model.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loan_Backend.Domain.Interface
{
    public interface IRepaymentPlanCalculator
    {
        RepaymentPlanDto ComputeRepaymentPlan(CustomerLoan loan);
    }
}
