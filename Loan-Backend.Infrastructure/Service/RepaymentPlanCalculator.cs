using Loan_Backend.Domain.Entities;
using Loan_Backend.Domain.Interface;
using Loan_Backend.SharedKernel;
using Loan_Backend.SharedKernel.Model.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loan_Backend.Infrastructure.Service
{
    public class RepaymentPlanCalculator : IRepaymentPlanCalculator
    {
        private const double AverageWeeksPerMonth = 4.2857;
        public RepaymentPlanDto ComputeRepaymentPlan(CustomerLoan loan)
        {
            var frequency = (InterestFrequencyEnum)Enum.Parse(typeof(InterestFrequencyEnum), loan.LoanGroup);

            int numberOfInstallments;
            int installmentIntervalDays;

            if (frequency == InterestFrequencyEnum.Weekly)
            {
                numberOfInstallments = loan.DurationInWeeks;
                installmentIntervalDays = 7;
            }
            else
            {
                numberOfInstallments = (int)Math.Ceiling(loan.DurationInWeeks / AverageWeeksPerMonth);
                installmentIntervalDays = 30;
            }

            var amountPerInstallment = loan.RepaymentAmount / numberOfInstallments;

            var repaymentDates = new List<DateTime>();
            for (int i = 1; i <= numberOfInstallments; i++)
            {
                var dueDate = loan.ApprovedDate.AddDays(i * installmentIntervalDays);
                repaymentDates.Add(dueDate);
            }

            return new RepaymentPlanDto
            {
                AmountPerInstallment = amountPerInstallment,
                DueDates = repaymentDates,
                NumberOfPaymentInstallments = numberOfInstallments
            };
        }
    }
}
