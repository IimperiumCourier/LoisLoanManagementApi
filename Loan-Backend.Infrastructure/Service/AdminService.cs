using Loan_Backend.Domain.Entities;
using Loan_Backend.Domain.Interface;
using Loan_Backend.SharedKernel;
using Loan_Backend.SharedKernel.Model.DTO;
using Loan_Backend.SharedKernel.Model.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loan_Backend.Infrastructure.Service
{
    public class AdminService : IAdminService
    {
        private readonly IUnitOfWork unitOfWork;
        public AdminService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;        
        }

        public async Task<ResponseWrapper<string>> ActivateAdmin(Guid adminId)
        {
            var admin = await unitOfWork.AdminRepository.GetByIdAsync(adminId);
            if(admin == null)
            {
                return ResponseWrapper<string>.Error("Admin record could not be activated");
            }

            admin.Activate();
            var count = await unitOfWork.SaveAsync();
            if(count <= 0)
            {
                return ResponseWrapper<string>.Error("Operation failed");
            }

            return ResponseWrapper<string>.Success("Operation was successful");
        }

        public async Task<ResponseWrapper<string>> ChangePassword(Guid adminId, string newPassword, string confirmedPassword)
        {
            var admin = await unitOfWork.AdminRepository.GetByIdAsync(adminId);
            if (admin == null)
            {
                return ResponseWrapper<string>.Error("Admin record could not be activated");
            }

            if(!string.Equals(newPassword, confirmedPassword))
            {
                return ResponseWrapper<string>.Error("Passwords do not match. Please try again.");
            }

            admin.ChangePassword(newPassword);
            var count = await unitOfWork.SaveAsync();
            if (count <= 0)
            {
                return ResponseWrapper<string>.Error("Operation failed");
            }

            return ResponseWrapper<string>.Success("Operation was successful");
        }

        public async Task<ResponseWrapper<AdminDto>> CreateAdmin(CreateAdminRequest request, Guid createdBy, bool isOperator = false)
        {
            var queryRequest = await unitOfWork.AdminRepository.FindAsync(e => e.EmailAddress.ToLower().Trim() == request.EmailAddress.ToLower().Trim() ||
                                                                                e.PhoneNumber == request.PhoneNumber);
            if(queryRequest == null)
            {
                return ResponseWrapper<AdminDto>.Error("Operation failed");
            }

            var adminRecordExists = queryRequest.FirstOrDefault() is not null;
            if (adminRecordExists)
            {
                return ResponseWrapper<AdminDto>.Error("Email or phone number provided is tied to a record");
            }

            var admin = Admin.Create(request.FullName, request.EmailAddress, request.PhoneNumber, request.Password);

            var roleName = isOperator ? "Operator" : "Admin";
            var roleEnumerable = await unitOfWork.RoleRepository.FindAsync(e => e.Name == roleName);
            var role = roleEnumerable.FirstOrDefault();

            if(role == null)
            {
                return ResponseWrapper<AdminDto>.Error($"{roleName} does not exist. Setup role to continue.");
            }

            await unitOfWork.AdminRoleRepository.AddAsync(admin.SetRole(role.Id, createdBy));
            await unitOfWork.AdminRepository.AddAsync(admin);

            var count = await unitOfWork.SaveAsync();
            if(count <= 0)
            {
                return ResponseWrapper<AdminDto>.Error("Operation failed");
            }

            return ResponseWrapper<AdminDto>.Success(new AdminDto
            {
                Id = admin.Id,
                EmailAddress = admin.EmailAddress,
                FullName = admin.FullName,
                IsActive = admin.IsActive,
                PhoneNumber = admin.PhoneNumber
            });
        }

        public async Task<ResponseWrapper<string>> DeactivateAdmin(Guid adminId)
        {
            var admin = await unitOfWork.AdminRepository.GetByIdAsync(adminId);
            if (admin == null)
            {
                return ResponseWrapper<string>.Error("Admin record could not be activated");
            }

            admin.Deactivate();
            var count = await unitOfWork.SaveAsync();
            if (count <= 0)
            {
                return ResponseWrapper<string>.Error("Operation failed");
            }

            return ResponseWrapper<string>.Success("Operation was successful");
        }

        public async Task<ResponseWrapper<PagedResult<AdminDto>>> GetAllUsersWithSpecifiedRole(bool isOperator, int pageNum, int pageSize)
        {
            var roleName = isOperator ? "Operator" : "Admin";
            var roleEnumerable = await unitOfWork.RoleRepository.FindAsync(e => e.Name == roleName);
            var role = roleEnumerable.FirstOrDefault();

            if (role == null)
            {
                return ResponseWrapper<PagedResult<AdminDto>>.Error($"{roleName} does not exist. Setup role to continue.");
            }

            var adminRoles = await unitOfWork.AdminRoleRepository.FindAsync(e => e.RoleId == role.Id);
            var adminIds = adminRoles.Select(e => e.RoleId).ToHashSet();
            if(adminIds == null)
            {
                return ResponseWrapper<PagedResult<AdminDto>>.Error($"There are no users with role {roleName}.");
            }

            var admins = await unitOfWork.AdminRepository.FindAsync(e => adminIds.Contains(e.Id));
            if (admins == null)
            {
                return ResponseWrapper<PagedResult<AdminDto>>.Error("No record found.");
            }

            int totalCount = admins.Count();

            var items = admins
                .Skip((pageNum - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var pagedResult = new PagedResult<AdminDto>
            {
                Items = items.Select(e => new AdminDto
                {
                    Id = e.Id,
                    EmailAddress = e.EmailAddress,
                    FullName = e.FullName,
                    IsActive = e.IsActive,
                    PhoneNumber = e.PhoneNumber
                }).ToList(),
                TotalCount = totalCount,
                PageNumber = pageNum,
                PageSize = pageSize
            };

            return ResponseWrapper<PagedResult<AdminDto>>.Success(pagedResult);
        }
    }
}
