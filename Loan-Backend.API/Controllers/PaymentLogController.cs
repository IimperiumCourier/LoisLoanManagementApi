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
        [HasPermission("can_log_payment")]
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
        [Route("loan/{loanId}/pagenumber/{pagenumber}/pagesize/{pagesize}")]
        [HasPermission("can_view_payment_log")]
        public async Task<ActionResult> GetPaymentLogByLoanId(Guid loanId, int pagenumber, int pagesize)
        {
            var response = await paymentLogService.GetPaymentLogUsingLoanId(loanId, pagenumber, pagesize);

            if (!response.IsSuccessful)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet]
        [Route("pagenumber/{pagenumber}/pagesize/{pagesize}")]
        [HasPermission("can_view_payment_log")]
        public async Task<ActionResult> GetPaymentLogPendingApproval(int pagenumber, int pagesize)
        {
            var response = await paymentLogService.GetPaymentLogsPendingApproval(pagenumber, pagesize);

            if (!response.IsSuccessful)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost]
        [Route("{paymentLogId}/approve")]
        [HasPermission("can_approve_payment")]
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
        [HasPermission("can_approve_payment")]
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
