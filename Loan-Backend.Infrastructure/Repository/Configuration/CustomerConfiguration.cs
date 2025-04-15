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
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.Property(e => e.Id).IsRequired();
            builder.Property(e => e.FullName).HasMaxLength(100);
            builder.Property(e => e.Email).HasMaxLength(50).IsRequired();
            builder.Property(e => e.Phonenumber).HasMaxLength(20).IsRequired();
            builder.Property(e => e.MaritalStatus).HasMaxLength(20).IsRequired(false);
            builder.Property(e => e.Gender).HasMaxLength(20);
            builder.Property(e => e.ResidentialAddress).HasMaxLength(200).IsRequired();
            builder.Property(e => e.EmploymentStatus).HasMaxLength(200).IsRequired();
            builder.Property(e => e.SelfieUrl).HasMaxLength(200).IsRequired(false);
            builder.Property(e => e.MonthlyIncome).IsRequired();
            builder.OwnsOne(e => e.Guarantor, a =>
            {
                a.Property(p => p.EmailAddress).HasMaxLength(50).IsRequired();
                a.Property(p => p.ResidentialAddress).HasMaxLength(200).IsRequired();
                a.Property(p => p.PhoneNumber).HasMaxLength(20).IsRequired();
                a.Property(p => p.RelationshipToCustomer).HasMaxLength(20).IsRequired();
                a.Property(p => p.IdentificationNumber).HasMaxLength(50).IsRequired();
                a.Property(p => p.IdentificationType).HasMaxLength(50).IsRequired();
                a.Property(p => p.IdentificationUrl).IsRequired(false);
            });

            builder.OwnsOne(e => e.Identification, a =>
            {
                a.Property(p => p.IdentificationNumber).HasMaxLength(50).IsRequired();
                a.Property(p => p.IdentificationType).HasMaxLength(50).IsRequired();
                a.Property(p => p.IdentificationUrl).IsRequired(false);
            });
        }
    }
}
