using Loan_Backend.Domain.Interface;
using Loan_Backend.SharedKernel;
using Loan_Backend.SharedKernel.Model.Request;
using Loan_Backend.SharedKernel.Model.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loan_Backend.Infrastructure.Service
{
    public class AuthenticationService(IUnitOfWork unitOfWork, ITokenGenerator tokenGenerator) : IAuthenticationService
    {

        public async Task<ResponseWrapper<AuthenticationResponse>> Authenticate(AuthenticationRequest request)
        {
            var email = request.Email.ToLower().Trim();
            var queryResult = await unitOfWork.AdminRepository.FindAsync(e => e.EmailAddress == email);

            if(queryResult == null)
            {
                return ResponseWrapper<AuthenticationResponse>.Error("Admin record not found");
            }

            var admin = queryResult.FirstOrDefault();
            if (admin == null)
            {
                return ResponseWrapper<AuthenticationResponse>.Error("Admin record not found");
            }

            var (isPasswordRetryCountExceeded, message) = admin.CheckRetryCount();
            if (isPasswordRetryCountExceeded)
            {
                return ResponseWrapper<AuthenticationResponse>.Error(message);
            }

            var (isValid, msg) = admin.IsPasswordValid(request.Password);
            if (!isValid)
            {
                await unitOfWork.AdminRepository.UpdateAsync(admin);
                await unitOfWork.SaveAsync();

                return ResponseWrapper<AuthenticationResponse>.Error(msg);
            }

            var adminRoles = await unitOfWork.AdminRoleRepository.FindAsync(e => e.AdminId == admin.Id);
            var adminRole = adminRoles.FirstOrDefault();
            if (adminRole == null)
            {
                return ResponseWrapper<AuthenticationResponse>.Error("Admin role record not found");
            }

            var role = await unitOfWork.RoleRepository.GetByIdAsync(adminRole.RoleId);
            if (role == null)
            {
                return ResponseWrapper<AuthenticationResponse>.Error("Role record not found");
            }

            var rolePermissionEnumeration = await unitOfWork.RolePermissionRepository.FindAsync(e => e.RoleId == adminRole.RoleId && e.IsActive);
            var rolePermissions = rolePermissionEnumeration.Select(e => e.PermissionId).ToList();
            if(rolePermissions == null)
            {
                return ResponseWrapper<AuthenticationResponse>.Error("Role permissions not found");
            }

            var permissionEnumeration = await unitOfWork.PermissionRepository.GetAllAsync();
            var allPemissions = permissionEnumeration.ToList();
            var permissions = allPemissions.Where(e => rolePermissions.Contains(e.Id) && e.IsActive).Select(e => e.Name).ToList();
            if (permissions == null)
            {
                return ResponseWrapper<AuthenticationResponse>.Error("Permissions not found");
            }

            var jwtToken = tokenGenerator.GenerateToken(admin.Id.ToString(), admin.FullName, role.Name, permissions);

            var refreshToken = admin.SetRefreshToken();
            await unitOfWork.AdminRepository.UpdateAsync(admin);
            await unitOfWork.SaveAsync();

            return ResponseWrapper<AuthenticationResponse>.Success(new AuthenticationResponse
            {
                FullName = admin.FullName,
                JWT = jwtToken,
                JwtRefreshToken = refreshToken,
                Role = role.Name
            });
        }

        public async Task<ResponseWrapper<string>> RefreshToken(string refreshToken)
        {
            var queryResult = await unitOfWork.AdminRepository.FindAsync(e => e.RefreshToken == refreshToken);

            if (queryResult == null)
            {
                return ResponseWrapper<string>.Error("Refresh token is invalid or has expired. Kindly login to continue.");
            }

            var admin = queryResult.FirstOrDefault();
            if (admin == null || admin.RefreshTokenExpired())
            {
                return ResponseWrapper<string>.Error("Refresh token is invalid or has expired. Kindly login to continue.");
            }

            var adminRoles = await unitOfWork.AdminRoleRepository.FindAsync(e => e.AdminId == admin.Id);
            var adminRole = adminRoles.FirstOrDefault();
            if (adminRole == null)
            {
                return ResponseWrapper<string>.Error("Admin role record not found");
            }

            var role = await unitOfWork.RoleRepository.GetByIdAsync(adminRole.RoleId);
            if (role == null)
            {
                return ResponseWrapper<string>.Error("Role record not found");
            }

            var rolePermissionEnumeration = await unitOfWork.RolePermissionRepository.FindAsync(e => e.RoleId == adminRole.RoleId && e.IsActive);
            var rolePermissions = rolePermissionEnumeration.Select(e => e.PermissionId).ToList();
            if (rolePermissions == null)
            {
                return ResponseWrapper<string>.Error("Role permissions not found");
            }

            var permissionEnumeration = await unitOfWork.PermissionRepository.GetAllAsync();
            var allPemissions = permissionEnumeration.ToList();
            var permissions = allPemissions.Where(e => rolePermissions.Contains(e.Id) && e.IsActive).Select(e => e.Name).ToList();
            if (permissions == null)
            {
                return ResponseWrapper<string>.Error("Permissions not found");
            }

            var jwtToken = tokenGenerator.GenerateToken(admin.Id.ToString(), admin.FullName, role.Name, permissions);

            return ResponseWrapper<string>.Success(jwtToken);
        }
    }
}
