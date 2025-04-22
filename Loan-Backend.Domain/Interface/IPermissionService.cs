using Loan_Backend.Domain.Entities;
using Loan_Backend.SharedKernel.Model.DTO;
using Loan_Backend.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loan_Backend.Domain.Interface
{
    public interface IPermissionService
    {
        Task<ResponseWrapper<Permission>> Create(string permissionName);
        Task<ResponseWrapper<PagedResult<Permission>>> GetAllPermissions(int pageNumber, int pageSize);
        Task<ResponseWrapper<Permission>> GetPermission(Guid permissionId);
        Task<ResponseWrapper<string>> DeactivatePermission(Guid permissionId);
        Task<ResponseWrapper<string>> ActivatePermission(Guid permissionId);
        Task<ResponseWrapper<string>> AddPermissionToRoles(Guid permissionId, List<Guid> roleIds, Guid grantorId);
    }
}
