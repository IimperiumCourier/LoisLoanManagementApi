using OfficeOpenXml;
using OfficeOpenXml.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loan_Backend.SharedKernel
{
    public class ExcelReader
    {

        public static ExcelWorksheet GetWorkSheet(string rootPath, string folder, string fileName)
        {
            var filePath = Path.Combine(rootPath, folder, fileName);
            if (!File.Exists(filePath)) return null!;

            ExcelPackage.License.SetNonCommercialPersonal("Lois API");
            using var package = new ExcelPackage(new FileInfo(filePath));
            var worksheet = package.Workbook.Worksheets[0];

            return worksheet;
        }
    }
}
