using Loan_Backend.SharedKernel;
using Loan_Backend.SharedKernel.Model.Request;
using Loan_Backend.SharedKernel.Model.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loan_Backend.Domain.Interface
{
    public interface IProfitAnalyticsStrategy
    {
        Task<ResponseWrapper<ProfitAnalyticsResult>> GetProfitAnalysis(ProfitAnalyticsRequest request);
    }

}
