using Loan_Backend.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loan_Backend.Domain.Interface
{
    public interface IUnitOfWork : IDisposable
    {
        IBaseRepository<Customer> CustomerRepository { get; }
        IBaseRepository<CustomerLoan> CustomerLoanRepository { get; }
        IBaseRepository<Admin> AdminRepository { get; }
        IBaseRepository<AdminRole> AdminRoleRepository { get; }
        IBaseRepository<RolePermission> RolePermissionRepository { get; }
        IBaseRepository<Permission> PermissionRepository { get; }
        IBaseRepository<PaymentLog> PaymentLogRepository { get; }
        IBaseRepository<LoginHistory> LoginHistoryRepository { get; }
        IBaseRepository<Role> RoleRepository { get; }
        Task<int> SaveAsync();
    }
}
