using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Loan_Backend.Infrastructure.Repository.Context;
using Loan_Backend.SharedKernel;
using System.Xml;
using Loan_Backend.Domain.Entities;

namespace Loan_Backend.Infrastructure.Repository
{
    public class DataSeeder
    {
        private readonly LoanAppDbContext _context;
        private readonly IHostingEnvironment _env;
        public DataSeeder(LoanAppDbContext dbContext, IHostingEnvironment hostingEnv)
        {
            _context = dbContext;
            _env = hostingEnv;
        }

        public async Task SeedRolesFromExcelIfEmptyAsync()
        {
            var csvRows = ExcelReader.GetCsvRows(AppContext.BaseDirectory, "SeedFiles", "roles.csv");

            if (csvRows == null || csvRows.Count == 0) return;

            foreach (var row in csvRows)
            {
                if (row.Length == 0 || string.IsNullOrWhiteSpace(row[0])) continue;

                var roleName = row[0].ToTrimmedAndLowerCase();

                if (await _context.Roles.AnyAsync(e => e.Name == roleName))
                {
                    continue;
                }

                var entity = Role.Create(roleName);
                _context.Roles.Add(entity);
            }

            await _context.SaveChangesAsync();
        }

        public async Task SeedPermissionsFromExcelIfEmptyAsync()
        {
            var csvRows = ExcelReader.GetCsvRows(AppContext.BaseDirectory, "SeedFiles", "permissions.csv");

            if (csvRows == null || csvRows.Count == 0) return;

            foreach (var permission in csvRows)
            {
                if (permission.Length == 0 || string.IsNullOrWhiteSpace(permission[0])) continue;

                var permissionName = permission[0].ToTrimmedAndLowerCase();

                if (await _context.Permissions.AnyAsync(e => e.Name == permissionName))
                {
                    continue;
                }

                var entity = Permission.Create(permissionName);
                _context.Permissions.Add(entity);
            }

            await _context.SaveChangesAsync();
        }

        public async Task InsertRolePermissionsAsync()
        {
            var rows = ExcelReader.GetCsvRows(AppContext.BaseDirectory, "SeedFiles", "role_permissions.csv");
            if (rows == null || rows.Count == 0) return;

            // Cache roles and permissions to avoid repeated DB hits
            var roles = _context.Roles.ToDictionary(r => r.Name.ToLower(), r => r.Id);
            var permissions = _context.Permissions.ToDictionary(p => p.Name.ToLower(), p => p.Id);

            foreach (var row in rows)
            {
                if (row.Length < 2) continue;

                string roleName = row[0].Trim().ToLower();
                string[] permissionNames = row[1].Split('|', StringSplitOptions.RemoveEmptyEntries);

                if (!roles.TryGetValue(roleName, out var roleId)) continue;

                foreach (var permissionNameRaw in permissionNames)
                {
                    var permissionName = permissionNameRaw.Trim().ToLower();
                    if (!permissions.TryGetValue(permissionName, out var permissionId)) continue;

                    // Avoid duplicates if needed
                    bool exists = _context.RolePermissions
                        .Any(rp => rp.RoleId == roleId && rp.PermissionId == permissionId);

                    if (!exists)
                    {
                        _context.RolePermissions.Add(RolePermission.Create(roleId,permissionId,Guid.Empty));
                    }
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task InsertAdminsFromCsv()
        {
            var rows = ExcelReader.GetCsvRows(AppContext.BaseDirectory, "SeedFiles", "admins.csv");
            if (rows == null || rows.Count == 0) return;

            var roles = _context.Roles.ToDictionary(r => r.Name.ToLower(), r => r.Id);

            foreach (var row in rows)
            {
                if (row.Length < 5) continue;

                var fullName = row[0].Trim();
                var email = row[1].Trim().ToLower();
                var phone = row[2].Trim();
                var password = row[3].Trim();
                var roleName = row[4].Trim().ToLower();

                if (!roles.TryGetValue(roleName, out var roleId)) continue;

                if (_context.Admins.Any(a => a.EmailAddress == email)) continue;


                var admin = Admin.Create(fullName,email,phone,password);

                var adminRole = AdminRole.Create(admin.Id,roleId,Guid.Empty);

                _context.Admins.Add(admin);
                _context.AdminRoles.Add(adminRole);
            }

            await _context.SaveChangesAsync();
        }

    }
}
