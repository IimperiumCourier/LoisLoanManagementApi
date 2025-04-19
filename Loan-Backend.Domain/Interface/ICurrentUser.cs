using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loan_Backend.Domain.Interface
{
    public interface ICurrentUser
    {
        string? Id { get; }
        string? Role { get; }
        string? Name { get; }
        bool IsAuthenticated { get; }
    }
}
