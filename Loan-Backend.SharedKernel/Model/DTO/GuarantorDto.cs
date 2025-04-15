using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loan_Backend.SharedKernel.Model.DTO
{
    public class GuarantorDto
    {
        public string FullName { get; set; } = string.Empty;
        public string RelationshipToCustomer { get; set; } = string.Empty;
        public string ResidentialAddress { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string EmailAddress { get; set; } = string.Empty;
        public IdentificationDto? Identification { get; set; }
    }
}
