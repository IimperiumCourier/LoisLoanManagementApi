using Loan_Backend.SharedKernel.Model.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loan_Backend.Domain.ValueObjects
{
    public class Identification : BaseValueObject<Identification>
    {
        public string IdentificationType { get; private set; } = string.Empty;
        public string IdentificationNumber { get; private set; } = string.Empty;
        public string IdentificationUrl { get; private set; } = string.Empty;

        internal static Identification Create(IdentificationDto identificationDto)
        {
            return new Identification
            {
                IdentificationNumber = identificationDto.IdentificationNumber,
                IdentificationType = identificationDto.IdentificationType,
                IdentificationUrl = identificationDto.IdentificationUrl
            };
        }
    }
}
