using Loan_Backend.Domain.Entities;
using Loan_Backend.SharedKernel;
using Loan_Backend.SharedKernel.Model.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loan_Backend.Domain.Interface
{
    public interface IRoleService
    {
        Task<ResponseWrapper<Role>> Create(string roleName);
        Task<ResponseWrapper<PagedResult<Role>>> GetAllRoles(int pageNumber, int pageSize);
        Task<ResponseWrapper<Role>> GetRoleById(Guid roleId);
        Task<ResponseWrapper<PagedResult<Permission>>> GetRolePermissions(Guid roleId, int pageNumber, int pageSize);
        Task<ResponseWrapper<string>> DeactivateRole(Guid roleId);
        Task<ResponseWrapper<string>> ActivateRole(Guid roleId);
        Task<ResponseWrapper<string>> AddRolePermissions(Guid roleId, List<Guid> permissionIds);
    }
}
