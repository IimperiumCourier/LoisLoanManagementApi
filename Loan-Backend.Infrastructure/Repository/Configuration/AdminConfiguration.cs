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
    public class AdminConfiguration: IEntityTypeConfiguration<Admin>
    {
        public void Configure(EntityTypeBuilder<Admin> builder)
        {
            builder.Property(e => e.Id).IsRequired();
            builder.Property(e => e.FullName).HasMaxLength(100);
            builder.Property(e => e.EmailAddress).HasMaxLength(50);
            builder.Property(e => e.RefreshToken).HasMaxLength(100).IsRequired(false);
            builder.Property(e => e.PhoneNumber).HasMaxLength(20).IsRequired(false);
        }
    }
}
