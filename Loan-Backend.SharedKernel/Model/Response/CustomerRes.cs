using System;
using System.Collections.Generic; // Make sure this is included for List<T>
using System.Linq; // Typically needed for LINQ operations like .ToList()

namespace Loan_Backend.SharedKernel.Model.Response // Adjust namespace as per your project structure
{
    /// <summary>
    /// Represents a detailed customer response DTO, including personal, contact,
    /// employment, identification, guarantor, and associated loan IDs.
    /// </summary>
    public class CustomerRes
    {
        public Guid Id { get; set; }
        public string? FullName { get; set; }
        public string? Phonenumber { get; set; }
        public string? Email { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? MaritalStatus { get; set; }
        public string? ResidentialAddress { get; set; }
        public string? EmploymentStatus { get; set; }
        public decimal MonthlyIncome { get; set; }
        public string? SelfieUrl { get; set; }

        public Identification? Identification { get; set; }
        public Guarantor? Guarantor { get; set; }
        public LoanPreference? LoanPreference { get; set; }

        // === Property to hold a list of all loan IDs for this customer ===
        public List<Guid>? LoanIds { get; set; } // Now a list of Guids, can be null if no loans
    }

    // Nested classes (Identification, Guarantor, LoanPreference) remain the same
    public class Identification
    {
        public string? IdentificationType { get; set; }
        public string? IdentificationNumber { get; set; }
        public string? IdentificationUrl { get; set; }
    }

    public class Guarantor
    {
        public string? FullName { get; set; }
        public string? RelationshipToCustomer { get; set; }
        public string? PhoneNumber { get; set; }
        public string? EmailAddress { get; set; }
        public Identification? Identification { get; set; } // This is expected to be null if your entity Guarantor doesn't have it
        public string? ResidentialAddress { get; set; }
    }

    public class LoanPreference
    {
        public string? LoanGroup { get; set; }
        public decimal Amount { get; set; }
        public string? CurrencyCode { get; set; }
        public decimal InterestRate { get; set; }
        public int DurationInWeeks { get; set; }
    }
}
