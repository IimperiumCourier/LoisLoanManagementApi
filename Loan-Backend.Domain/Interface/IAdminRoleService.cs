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
    public interface IAdminRoleService
    {
        Task<ResponseWrapper<AdminRole>> Create(Guid adminId, Guid roleId, Guid grantorId);
        Task<ResponseWrapper<AdminRole>> GetAdminRole(Guid adminId);
        Task<ResponseWrapper<PagedResult<Admin>>> GetAdminsByRoleId(Guid roleId, int pageNumber, int pageSize);
        Task<ResponseWrapper<string>> DeactivateAdminRole(Guid adminRoleId);
        Task<ResponseWrapper<string>> ActivateAdminRole(Guid adminRoleId);
    }
}
