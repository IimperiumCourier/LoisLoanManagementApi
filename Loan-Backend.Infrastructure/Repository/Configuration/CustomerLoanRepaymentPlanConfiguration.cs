using Loan_Backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loan_Backend.Infrastructure.Repository.Configuration
{
    public class CustomerLoanRepaymentPlanConfiguration : IEntityTypeConfiguration<CustomerLoanRepaymentPlan>
    {
        public void Configure(EntityTypeBuilder<CustomerLoanRepaymentPlan> builder)
        {
            builder.Property(e => e.Id).IsRequired();
            builder.Property(e => e.LoanId).IsRequired();
            builder.Property(e => e.DueDate).IsRequired();
        }
    }
}
