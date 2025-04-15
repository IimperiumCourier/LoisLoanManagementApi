using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loan_Backend.Domain.Interface
{
    public interface ITokenGenerator
    {
        string GenerateToken(string userId, string name, string role, List<string> permissions);
    }
}
