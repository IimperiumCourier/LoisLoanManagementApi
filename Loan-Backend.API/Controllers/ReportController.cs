using Loan_Backend.Infrastructure.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Loan_Backend.API.Controllers
{
    [Route("api/report")]
    [ApiController]
    public class ReportController : ControllerBase
    {

        public ReportController()
        {
                
        }

        [HttpGet]
        [Route("profit/analytics")]
        public async Task<ActionResult> GetAllOperators(int pagenumber, int pagesize)
        {
            var response = await adminService.GetAllUsersWithSpecifiedRole(true, pagenumber, pagesize);
            if (!response.IsSuccessful)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}
