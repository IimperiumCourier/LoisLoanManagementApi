using Loan_Backend.API.Attribute;
using Loan_Backend.Domain.Interface;
using Loan_Backend.Infrastructure.Service;
using Loan_Backend.SharedKernel.Model.DTO;
using Loan_Backend.SharedKernel.Model.Request;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Loan_Backend.API.Controllers
{
    [Route("api/paymentlog")]
    [ApiController]
    public class PaymentLogController : ControllerBase
    {
        private readonly IPaymentLogService paymentLogService;
        private readonly ICurrentUser currentUser;
        public PaymentLogController(IPaymentLogService paymentLogService, ICurrentUser currentUser)
        {
            this.paymentLogService = paymentLogService;
            this.currentUser = currentUser;
        }

        [HttpPost]
        [HasPermission("CanLogPayment")]
        public async Task<ActionResult> LogPayment(CreatePaymentLogReq request)
        {
            var response = await paymentLogService.LogPayment(request, currentUser.Id!);

            if (!response.IsSuccessful)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet]
        [Route("loan/{loanId}/pageNumber/{pageNumber}/pagesize/{pageSize}")]
        [HasPermission("CanViewPaymentLog")]
        public async Task<ActionResult> GetPaymentLogByLoanId(Guid loanId, int pageNumber, int pageSize)
        {
            var response = await paymentLogService.GetPaymentLogUsingLoanId(loanId, pageNumber, pageSize);

            if (!response.IsSuccessful)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet]
        [Route("pageNumber/{pageNumber}/pagesize/{pageSize}")]
        [HasPermission("CanViewPaymentLog")]
        public async Task<ActionResult> GetPaymentLogPendingApproval(int pageNumber, int pageSize)
        {
            var response = await paymentLogService.GetPaymentLogsPendingApproval(pageNumber, pageSize);

            if (!response.IsSuccessful)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost]
        [Route("{paymentLogId}/approve")]
        [HasPermission("CanApprovePayment")]
        public async Task<ActionResult> ApprovePayment(Guid paymentLogId)
        {
            var response = await paymentLogService.ApprovePayment(paymentLogId, currentUser.Id!);

            if (!response.IsSuccessful)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost]
        [Route("{paymentLogId}/decline")]
        [HasPermission("CanApprovePayment")]
        public async Task<ActionResult> DeclinePayment(Guid paymentLogId)
        {
            var response = await paymentLogService.DeclinePayment(paymentLogId, currentUser.Id!);

            if (!response.IsSuccessful)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}
