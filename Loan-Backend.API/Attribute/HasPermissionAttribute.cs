using Loan_Backend.API.Filter;
using Microsoft.AspNetCore.Mvc;

namespace Loan_Backend.API.Attribute
{
    public class HasPermissionAttribute : TypeFilterAttribute
    {
        public HasPermissionAttribute(string permission)
            : base(typeof(PermissionFilter))
        {
            Arguments = [permission];
        }
    }

}
