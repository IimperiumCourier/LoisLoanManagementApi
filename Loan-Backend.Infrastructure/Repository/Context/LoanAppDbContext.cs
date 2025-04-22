using Loan_Backend.Domain.Entities;
using Loan_Backend.Infrastructure.Repository.Configuration;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loan_Backend.Infrastructure.Repository.Context
{
    public class LoanAppDbContext : DbContext
    {
        public LoanAppDbContext(DbContextOptions<LoanAppDbContext> options) : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<CustomerLoan> CustomerLoans { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<AdminRole> AdminRoles { get; set; }
        public DbSet<PaymentLog> PaymentLogs { get; set; }
        public DbSet<Role> Roles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new AdminConfiguration());
            modelBuilder.ApplyConfiguration(new CustomerConfiguration());
            modelBuilder.ApplyConfiguration(new CustomerLoanConfiguration());
            modelBuilder.ApplyConfiguration(new PaymentLogConfiguration());
            modelBuilder.ApplyConfiguration(new PermissionConfiguration());
            modelBuilder.ApplyConfiguration(new RoleConfiguration());
        }
    }
}
