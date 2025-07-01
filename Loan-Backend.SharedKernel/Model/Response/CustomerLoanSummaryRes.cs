using System;
using System.Collections.Generic;

namespace Loan_Backend.SharedKernel.Model.Response // Ensure this namespace matches your other DTOs
{
    /// <summary>
    /// Represents a summary of customer details including their associated loan IDs.
    /// Designed for lightweight retrieval of customer name and loan identifiers.
    /// </summary>
    public class CustomerLoanSummaryRes
    {
        /// <summary>
        /// Gets or sets the unique identifier of the customer.
        /// </summary>
        public Guid CustomerId { get; set; }

        /// <summary>
        /// Gets or sets the full name of the customer.
        /// </summary>
        public string? FullName { get; set; }

        /// <summary>
        /// Gets or sets a list of unique identifiers for all loans associated with this customer.
        /// Can be null or empty if the customer has no loans.
        /// </summary>
        public List<Guid>? LoanIds { get; set; }
    }
}
