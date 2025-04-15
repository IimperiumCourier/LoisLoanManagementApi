using Loan_Backend.Domain.ValueObjects;
using Loan_Backend.SharedKernel;
using Loan_Backend.SharedKernel.Model.DTO;
using Loan_Backend.SharedKernel.Model.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loan_Backend.Domain.Entities
{
    public class Customer : BaseEntity<Guid>
    {
        public string FullName { get; private set; } = string.Empty;
        public DateTime DateOfBirth { get; private set; }
        public string Email { get; private set; } = string.Empty;
        public string Phonenumber { get; private set; } = string.Empty;
        public string MaritalStatus { get; private set; } = string.Empty;
        public string Gender { get; private set; } = string.Empty;
        public string ResidentialAddress { get; private set; } = string.Empty;
        public string EmploymentStatus { get; private set; } = string.Empty;
        public decimal MonthlyIncome { get; private set; }
        public string SelfieUrl { get; private set; }
        public Guarantor Guarantor { get; private set; }
        public Identification Identification { get; private set; }

        public Customer(Guid id):base(id)
        {
                
        }

        public Customer():base(Guid.NewGuid())
        {
                
        }

        public static Customer Create(CreateCustomerReq request)
        {
            var customer = new Customer
            {
                DateOfBirth = request.DateOfBirth,
                Email = request.Email.ToLower(),
                EmploymentStatus = request.EmploymentStatus.ToProperCase(),
                FullName = request.FullName.ToProperCase(),
                Gender = request.Gender.ToProperCase(),
                MaritalStatus = request.MaritalStatus.ToProperCase(),
                MonthlyIncome = request.MonthlyIncome,
                Phonenumber = request.Phonenumber,
                ResidentialAddress = request.ResidentialAddress,
                SelfieUrl = request.SelfieUrl
            };

            if(request.Guarantor != null)
            {

                customer.Guarantor = Guarantor.Create(request.Guarantor);
            }

            if(request.Identification != null)
            {
                customer.Identification = Identification.Create(request.Identification);
            }

            return customer;
        }

        public CustomerLoan CreateLoan(LoanPreference loanPreference, string createdBy)
        {
            return CustomerLoan.Create(Id, createdBy, loanPreference);
        }
    }
}
