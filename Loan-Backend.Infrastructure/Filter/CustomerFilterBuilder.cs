using Loan_Backend.Domain.Entities;
using Loan_Backend.SharedKernel;
using Loan_Backend.SharedKernel.Model.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Loan_Backend.Infrastructure.Filter
{
    public static class CustomerFilterBuilder
    {
        public static Expression<Func<Customer, bool>> Build(CustomerFilter filter)
        {
            var predicate = PredicateBuilder.True<Customer>();

            if (filter.UseName)
            {
                predicate = predicate.And(c => c.FullName.Contains(filter.SearchKeyword.ToProperCase()));
            }

            if (filter.UseEmail)
            {
                predicate = predicate.And(c => c.Email.Contains(filter.SearchKeyword.ToLower()));
            }

            if (filter.UsePhoneNumber)
            {
                predicate = predicate.And(c => c.Phonenumber.Contains(filter.SearchKeyword.ToLower()));
            }

            return predicate;
        }
    }

}
