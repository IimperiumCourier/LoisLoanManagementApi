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

        public static List<string[]> GetCsvRows(string rootPath, string folder, string fileName)
        {
            var filePath = Path.Combine(rootPath, folder, fileName);
            if (!File.Exists(filePath)) return null!;

            var rows = new List<string[]>();

            using var reader = new StreamReader(filePath);

            // Skip the header row
            reader.ReadLine();

            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (line == null) continue;

                var values = line.Split(',');
                rows.Add(values);
            }

            return rows;
        }

    }
}
