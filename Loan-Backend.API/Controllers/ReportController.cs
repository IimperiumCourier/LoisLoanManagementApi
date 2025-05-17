using Loan_Backend.Domain.Interface;
using Loan_Backend.Infrastructure.Service;
using Loan_Backend.SharedKernel.Model.DTO;
using Loan_Backend.SharedKernel;
using Loan_Backend.SharedKernel.Model.Request;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Loan_Backend.SharedKernel.Model.Response;

namespace Loan_Backend.API.Controllers
{
    [Route("api/report")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IReportService reportService;
        public ReportController(IReportService reportService)
        {
                this.reportService = reportService;
        }

        [HttpPost]
        [Route("profit/analytics")]
        [ProducesResponseType(typeof(ResponseWrapper<ProfitAnalyticsResult>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseWrapper<ProfitAnalyticsResult>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> GetAllOperators([FromBody] ProfitAnalyticsRequest request)
        {
            var response = await reportService.AnalyzeProfit(request);
            if (!response.IsSuccessful)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}
