using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loan_Backend.SharedKernel.Model.DTO
{
    public class CustomerFilter
    {
        public bool UseName { get; set; }
        public bool UseEmail { get; set; }
        public bool UsePhoneNumber { get; set; }
        public string SearchKeyword { get; set; } = string.Empty;
        public int PageNumber { get; set; } = Common.PageNumber;
        public int PageSize { get; set; } = Common.PageSize;
    }
}
