using Loan_Backend.Domain.Interface;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Loan_Backend.Infrastructure.Service
{
    public class CurrentUser : ICurrentUser
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUser(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

        public string? Id => User?.Claims.FirstOrDefault(e => e.Type == ClaimTypes.NameIdentifier)?.Value;

        public string? Role => User?.Claims.FirstOrDefault(e => e.Type == "role")?.Value;

        public string? Name => User?.Claims.FirstOrDefault(e => e.Type == ClaimTypes.Name)?.Value;

        public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;
    }
}
