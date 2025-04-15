using Loan_Backend.Domain.Entities;
using Loan_Backend.Domain.Interface;
using Loan_Backend.Infrastructure.Repository.Context;

namespace Loan_Backend.Infrastructure.Repository
{
    public class UnitOfWork(LoanAppDbContext context) : IUnitOfWork
    {

        public IBaseRepository<Customer> CustomerRepository => new BaseRepository<Customer>(context);
        public IBaseRepository<CustomerLoan> CustomerLoanRepository => new BaseRepository<CustomerLoan>(context);
        public IBaseRepository<Admin> AdminRepository => new BaseRepository<Admin>(context);
        public IBaseRepository<AdminRole> AdminRoleRepository => new BaseRepository<AdminRole>(context);
        public IBaseRepository<RolePermission> RolePermissionRepository => new BaseRepository<RolePermission>(context);
        public IBaseRepository<Permission> PermissionRepository => new BaseRepository<Permission>(context);
        public IBaseRepository<PaymentLog> PaymentLogRepository => new BaseRepository<PaymentLog>(context);
        public IBaseRepository<LoginHistory> LoginHistoryRepository => new BaseRepository<LoginHistory>(context);
        public IBaseRepository<Role> RoleRepository => new BaseRepository<Role>(context);

        public void Dispose()
        {
            context.Dispose();
            GC.SuppressFinalize(this);
        }

        public async Task<int> SaveAsync()
        {
            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var result = await context.SaveChangesAsync();
                await transaction.CommitAsync();
                return result;
            }catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
