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
    public class AdminRoleService : IAdminRoleService
    {
        private readonly IUnitOfWork unitOfWork;
        public AdminRoleService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<ResponseWrapper<string>> ActivateAdminRole(Guid adminRoleId)
        {
            var adminRole = await unitOfWork.AdminRoleRepository.GetByIdAsync(adminRoleId);
            if(adminRole == null)
            {
                return ResponseWrapper<string>.Error("Admin role was not found.");
            }

            adminRole.Acivate();
            await unitOfWork.AdminRoleRepository.UpdateAsync(adminRole);

            var count = await unitOfWork.SaveAsync();
            if(count <= 0)
            {
                return ResponseWrapper<string>.Error("Admin role was not activated successfully.");
            }

            return ResponseWrapper<string>.Success("Admin role was activated successfully.");
        }

        public async Task<ResponseWrapper<AdminRole>> Create(Guid adminId, Guid roleId, Guid grantorId)
        {
            var adminRoleEnum = await unitOfWork.AdminRoleRepository
                                            .FindAsync(e => e.AdminId == adminId && e.IsActive);
            var adminRole = adminRoleEnum.FirstOrDefault();
            if (adminRole != null)
            {
                return ResponseWrapper<AdminRole>.Error("Admin has been assigned an existing role");
            }

            adminRole = AdminRole.Create(adminId, roleId, grantorId);
            await unitOfWork.AdminRoleRepository.AddAsync(adminRole);
            var count = await unitOfWork.SaveAsync();

            if (count <= 0)
            {
                return ResponseWrapper<AdminRole>.Error("Role was not successfully assigned to admin.");
            }

            return ResponseWrapper<AdminRole>.Success(adminRole);
        }

        public async Task<ResponseWrapper<string>> DeactivateAdminRole(Guid adminRoleId)
        {
            var adminRole = await unitOfWork.AdminRoleRepository.GetByIdAsync(adminRoleId);
            if (adminRole == null)
            {
                return ResponseWrapper<string>.Error("Admin role was not found.");
            }

            adminRole.Deacivate();
            await unitOfWork.AdminRoleRepository.UpdateAsync(adminRole);

            var count = await unitOfWork.SaveAsync();
            if (count <= 0)
            {
                return ResponseWrapper<string>.Error("Admin role was not deactivated successfully.");
            }

            return ResponseWrapper<string>.Success("Admin role was deactivated successfully.");
        }

        public async Task<ResponseWrapper<AdminRole>> GetAdminRole(Guid adminId)
        {
            var adminRoleEnum = await unitOfWork.AdminRoleRepository
                                            .FindAsync(e => e.AdminId == adminId && e.IsActive);
            var adminRole = adminRoleEnum.FirstOrDefault();
            if (adminRole == null)
            {
                return ResponseWrapper<AdminRole>.Error("Admin has not been assigned a role");
            }

            return ResponseWrapper<AdminRole>.Success(adminRole);
        }

        public async Task<ResponseWrapper<PagedResult<Admin>>> GetAdminsByRoleId(Guid roleId, int pageNumber, int pageSize)
        {
            var adminEnumerable = await unitOfWork.AdminRoleRepository.FindAsync(e => e.RoleId == roleId && e.IsActive);
            var adminIds = adminEnumerable.Select(e => e.AdminId).ToHashSet();

            if(adminIds == null || adminIds.Count <= 0)
            {
                return ResponseWrapper<PagedResult<Admin>>.Error("No admins have been assigned the specified role");
            }

            var admins = await unitOfWork.AdminRepository.FindAsync(e => adminIds.Contains(e.Id));
            if (admins == null)
            {
                return ResponseWrapper<PagedResult<Admin>>.Error("No record found.");
            }

            int totalCount = admins.Count();

            var items = admins
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var pagedResult = new PagedResult<Admin>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            return ResponseWrapper<PagedResult<Admin>>.Success(pagedResult);
        }
    }
}
