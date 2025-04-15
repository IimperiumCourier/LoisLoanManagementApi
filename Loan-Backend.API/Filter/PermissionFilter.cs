using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using System.Security.Claims;

namespace Loan_Backend.API.Filter
{
    public class PermissionFilter : IAuthorizationFilter
    {
        private readonly string _requiredPermission;

        public PermissionFilter(string requiredPermission)
        {
            _requiredPermission = requiredPermission;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            if (!user.Identity?.IsAuthenticated ?? false)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var hasPermission = user.Claims
                .Where(c => c.Type == "permission")
                .Any(c => c.Value == _requiredPermission);

            if (!hasPermission)
            {
                context.Result = new ForbidResult();
            }
        }
    }

}
