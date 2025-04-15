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
    public class CustomerLoanConfiguration: IEntityTypeConfiguration<CustomerLoan>
    {
        public void Configure(EntityTypeBuilder<CustomerLoan> builder)
        {
            builder.Property(e => e.Id).IsRequired();
            builder.Property(e => e.ApprovedBy).HasMaxLength(100);
            builder.Property(e => e.Status).HasMaxLength(50);
            builder.Property(e => e.CurrencyCode).HasMaxLength(10).IsRequired(false);
            builder.Property(e => e.LoanGroup).HasMaxLength(10).IsRequired(false);
        }
    }
}
