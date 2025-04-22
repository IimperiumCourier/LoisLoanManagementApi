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
            var worksheet = ExcelReader.
                            GetWorkSheet(_env.ContentRootPath, "SeedFiles", "roles.xlsx");

            var rowCount = worksheet.Dimension.Rows;
            for (int row = 2; row <= rowCount; row++)
            {
                var roleName = worksheet.Cells[row, 1].Text.ToTrimmedAndLowerCase();
                if(await _context.Roles.AnyAsync(e => e.Name == roleName))
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
            var worksheet = ExcelReader.
                            GetWorkSheet(_env.ContentRootPath, "SeedFiles", "permissions.xlsx");

            var rowCount = worksheet.Dimension.Rows;
            for (int row = 2; row <= rowCount; row++)
            {
                var permissionName = worksheet.Cells[row, 1].Text.ToTrimmedAndLowerCase();
                if (await _context.Permissions.AnyAsync(e => e.Name == permissionName))
                {
                    continue;
                }

                var entity = Permission.Create(permissionName);

                _context.Permissions.Add(entity);
            }

            await _context.SaveChangesAsync();
        }

    }
}
