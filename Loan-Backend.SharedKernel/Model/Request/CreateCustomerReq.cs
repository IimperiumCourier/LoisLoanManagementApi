using Loan_Backend.SharedKernel.Model.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loan_Backend.SharedKernel.Model.Request
{
    public class CreateCustomerReq
    {
        public string FullName { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Phonenumber { get; set; } = string.Empty;
        public string MaritalStatus { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string ResidentialAddress { get; set; } = string.Empty;
        public string EmploymentStatus { get; set; } = string.Empty;
        public decimal MonthlyIncome { get; set; }
        public string SelfieUrl { get; set; } = string.Empty;
        public LoanPreference? LoanPreference { get; set; }
        public GuarantorDto? Guarantor { get; set; }
        public IdentificationDto? Identification { get; set; }
    }
}
