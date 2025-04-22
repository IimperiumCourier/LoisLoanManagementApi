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
    public class PermissionService : IPermissionService
    {
        private readonly IUnitOfWork unitOfWork;

        public PermissionService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<ResponseWrapper<string>> ActivatePermission(Guid permissionId)
        {
            var permission = await unitOfWork.PermissionRepository.GetByIdAsync(permissionId);
            if(permission == null)
            {
                return ResponseWrapper<string>.Error("Permission could not be found");
            }

            permission.Activate();
            await unitOfWork.PermissionRepository.UpdateAsync(permission);
            var count = await unitOfWork.SaveAsync();

            if(count <= 0)
            {
                return ResponseWrapper<string>.Error("Permission could not be activated");
            }

            return ResponseWrapper<string>.Success("Permission was activated successfully");
        }

        public async Task<ResponseWrapper<string>> AddPermissionToRoles(Guid permissionId, List<Guid> roleIds, Guid grantorId)
        {
            var permission = await unitOfWork.PermissionRepository.GetByIdAsync(permissionId);
            if (permission == null)
            {
                return ResponseWrapper<string>.Error("Permission could not be found");
            }

            var rolePermissions = permission.CreateRolePermission(roleIds, grantorId);
            
            foreach(var rolePermission in rolePermissions)
            {
                await unitOfWork.RolePermissionRepository.AddAsync(rolePermission);
            }

            var count = await unitOfWork.SaveAsync();
            if(count <= 0)
            {
                return ResponseWrapper<string>.Error("Operation was not completed successfully");
            }

            return ResponseWrapper<string>.Error("Permission added to specified roles");
        }

        public async Task<ResponseWrapper<Permission>> Create(string permissionName)
        {
            permissionName = permissionName.ToTrimmedAndLowerCase();

            var permissionCheck = await unitOfWork.PermissionRepository.FindAsync(e => e.Name == permissionName && e.IsActive);
            if (permissionCheck.Any())
            {
                return ResponseWrapper<Permission>.Success(permissionCheck.FirstOrDefault()!);
            }

            var permission = Permission.Create(permissionName);
            await unitOfWork.PermissionRepository.AddAsync(permission);
            var count = await unitOfWork.SaveAsync();

            if(count <= 0)
            {
                return ResponseWrapper<Permission>.Error("Permission could not be created");
            }

            return ResponseWrapper<Permission>.Error("Permission was created successfully");
        }

        public async Task<ResponseWrapper<string>> DeactivatePermission(Guid permissionId)
        {
            var permission = await unitOfWork.PermissionRepository.GetByIdAsync(permissionId);
            if (permission == null)
            {
                return ResponseWrapper<string>.Error("Permission could not be found");
            }

            permission.Activate();
            await unitOfWork.PermissionRepository.UpdateAsync(permission);
            var count = await unitOfWork.SaveAsync();

            if (count <= 0)
            {
                return ResponseWrapper<string>.Error("Permission could not be deactivated");
            }

            return ResponseWrapper<string>.Success("Permission was deactivated successfully");
        }

        public async Task<ResponseWrapper<PagedResult<Permission>>> GetAllPermissions(int pageNumber, int pageSize)
        {
            var permissionEnumerable = await unitOfWork.PermissionRepository.GetAllAsync();
            if (permissionEnumerable == null || !permissionEnumerable.Any())
            {
                return ResponseWrapper<PagedResult<Permission>>.Error("No record found.");
            }

            int totalCount = permissionEnumerable.Count();

            var items = permissionEnumerable
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var pagedResult = new PagedResult<Permission>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            return ResponseWrapper<PagedResult<Permission>>.Success(pagedResult);
        }

        public async Task<ResponseWrapper<Permission>> GetPermission(Guid permissionId)
        {
            var permission = await unitOfWork.PermissionRepository.GetByIdAsync(permissionId);
            if (permission == null)
            {
                return ResponseWrapper<Permission>.Error("Permission could not be found");
            }

            return ResponseWrapper<Permission>.Success(permission);
        }
    }
}
