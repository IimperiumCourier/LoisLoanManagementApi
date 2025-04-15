using Loan_Backend.SharedKernel;
using Loan_Backend.SharedKernel.Model.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loan_Backend.Domain.ValueObjects
{
    public class Guarantor : BaseValueObject<Guarantor>
    {
        public string FullName { get; private set; } = string.Empty;
        public string RelationshipToCustomer { get; private set; } = string.Empty;
        public string ResidentialAddress { get; private set; } = string.Empty;
        public string PhoneNumber { get; private set; } = string.Empty;
        public string EmailAddress { get; private set; } = string.Empty;
        public string IdentificationType { get; private set; } = string.Empty;
        public string IdentificationNumber { get; private set; } = string.Empty;
        public string IdentificationUrl { get; private set; } = string.Empty;

        internal static Guarantor Create(GuarantorDto guarantorDto)
        {
            var guarantor = new Guarantor
            {
                EmailAddress = guarantorDto.EmailAddress.ToLower(),
                FullName = guarantorDto.FullName.ToProperCase(),
                PhoneNumber = guarantorDto.PhoneNumber,
                RelationshipToCustomer = guarantorDto.RelationshipToCustomer,
                ResidentialAddress = guarantorDto.ResidentialAddress
            };

            if(guarantorDto.Identification != null)
            {
                guarantor.IdentificationUrl = guarantorDto.Identification.IdentificationUrl;
                guarantor.IdentificationNumber = guarantorDto.Identification.IdentificationNumber;
                guarantor.IdentificationType = guarantorDto.Identification.IdentificationType;
            }

            return guarantor;
        }
    }
}
