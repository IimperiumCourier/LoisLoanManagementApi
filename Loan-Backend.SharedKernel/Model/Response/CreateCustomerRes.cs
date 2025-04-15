using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loan_Backend.SharedKernel.Model.Response
{
    public class CreateCustomerRes
    {
        public Guid CustomerId { get; set; }
        public Guid CustomerLoanId { get; set; }

        public static CreateCustomerRes Create(Guid customerId, Guid customerLoanId)
        {
            return new CreateCustomerRes
            {
                CustomerId = customerId,
                CustomerLoanId = customerLoanId
            };
        }
    }
}
