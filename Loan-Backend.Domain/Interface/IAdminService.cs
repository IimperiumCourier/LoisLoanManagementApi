using Loan_Backend.Domain.Entities;
using Loan_Backend.SharedKernel;
using Loan_Backend.SharedKernel.Model.DTO;
using Loan_Backend.SharedKernel.Model.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loan_Backend.Domain.Interface
{
    public interface IAdminService
    {
        Task<ResponseWrapper<AdminDto>> CreateAdmin(CreateAdminRequest request,Guid createdBy,bool isOperator = false);
        Task<ResponseWrapper<string>> ChangePassword(Guid adminId, string newPassword, string confirmedPassword);
        Task<ResponseWrapper<string>> DeactivateAdmin(Guid adminId);
        Task<ResponseWrapper<string>> ActivateAdmin(Guid adminId);
        Task<ResponseWrapper<PagedResult<AdminDto>>> GetAllUsersWithSpecifiedRole(bool isOperator, int pageNum, int pageSize);
    }
}
