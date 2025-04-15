using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loan_Backend.SharedKernel.Model.Response
{
    public class AuthenticationResponse
    {
        public string FullName { get; set; }
        public string Role { get; set; }
        public string JWT { get; set; }
        public string JwtRefreshToken { get; set; }
    }
}
