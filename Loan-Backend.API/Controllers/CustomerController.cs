using Loan_Backend.API.Attribute;
using Loan_Backend.Domain.Entities;
using Loan_Backend.Domain.Interface;
using Loan_Backend.SharedKernel;
using Loan_Backend.SharedKernel.Model.DTO;
using Loan_Backend.SharedKernel.Model.Request;
using Loan_Backend.SharedKernel.Model.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using System.Security.Claims;
using System; // Added for Guid and DateTime
using System.Threading.Tasks; // Added for Task

namespace Loan_Backend.API.Controllers
{
    [Route("api/customer")]
    [ApiController]
    [Authorize]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService customerService;
        private readonly ICustomerLoanService customerLoanService;
        private readonly ICurrentUser currentUser;
        public CustomerController(ICustomerService customerService,
                                  ICustomerLoanService customerLoanService,
                                  ICurrentUser currentUser)
        {
            this.customerService = customerService;
            this.customerLoanService = customerLoanService;
            this.currentUser = currentUser;
        }

        [HttpPost]
        [ProducesResponseType(typeof(ResponseWrapper<CreateCustomerRes>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseWrapper<CreateCustomerRes>), StatusCodes.Status400BadRequest)]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<ActionResult> CreateCustomer(CreateCustomerReq request)
        {
            var userId = User.Claims.FirstOrDefault(e => e.Type == ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized(ResponseWrapper<CreateCustomerRes>.Error("Request is unauthorized."));
            }

            var response = await customerService.CreateCustomer(request, userId.Value);

            if (!response.IsSuccessful)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost]
        [Route("search")]
        [ProducesResponseType(typeof(ResponseWrapper<PagedResult<Customer>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseWrapper<PagedResult<Customer>>), StatusCodes.Status400BadRequest)]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<ActionResult> GetCustomer(CustomerFilter request)
        {
            // NOTE: The return type in ProducesResponseType here is Customer, but the service
            // GetCustomerByFilter returns CustomerRes. Ensure consistency.
            // For now, keeping as Customer for the ProducesResponseType as per your original code,
            // but the actual response will be CustomerRes as per CustomerService.
            var response = await customerService.GetCustomerByFilter(request);

            if (!response.IsSuccessful)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(ResponseWrapper<Customer>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseWrapper<Customer>), StatusCodes.Status400BadRequest)]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<ActionResult> GetCustomerById(Guid id)
        {
            var response = await customerService.GetCustomerById(id);

            if (!response.IsSuccessful)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost]
        [Route("{id}/delete")]
        [ProducesResponseType(typeof(ResponseWrapper<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseWrapper<string>), StatusCodes.Status400BadRequest)]
        [Consumes(MediaTypeNames.Application.Json)]
        [HasPermission("can_delete_customer_record")]
        public async Task<ActionResult> DeleteCustomer(Guid id)
        {
            var response = await customerService.DeleteCustomer(id);

            if (!response.IsSuccessful)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost]
        [Route("loan/{id}/approve")]
        [ProducesResponseType(typeof(ResponseWrapper<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseWrapper<string>), StatusCodes.Status400BadRequest)]
        [Consumes(MediaTypeNames.Application.Json)]
        [HasPermission("can_approve_loan")]
        public async Task<ActionResult> ApproveCustomerLoan(Guid id)
        {
            var response = await customerLoanService.ApproveLoan(id, currentUser.Id!);

            if (!response.IsSuccessful)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost]
        [Route("loan")]
        [ProducesResponseType(typeof(ResponseWrapper<CustomerLoan>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseWrapper<CustomerLoan>), StatusCodes.Status400BadRequest)]
        [Consumes(MediaTypeNames.Application.Json)]
        [HasPermission("can_initiate_loan")]
        public async Task<ActionResult> CreateCustomerLoan(CreateLoanReq request)
        {
            var response = await customerLoanService.CreateLoan(request, currentUser.Id!);

            if (!response.IsSuccessful)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost]
        [Route("loan/{id}/decline")]
        [ProducesResponseType(typeof(ResponseWrapper<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseWrapper<string>), StatusCodes.Status400BadRequest)]
        [Consumes(MediaTypeNames.Application.Json)]
        [HasPermission("can_decline_loan")]
        public async Task<ActionResult> DeclineCustomerLoan(Guid id)
        {
            var response = await customerLoanService.DeclineLoan(id);

            if (!response.IsSuccessful)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost]
        [Route("loan/{id}/defaulted")]
        [ProducesResponseType(typeof(ResponseWrapper<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseWrapper<string>), StatusCodes.Status400BadRequest)]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<ActionResult> SetDefaultedCustomerLoan(Guid id)
        {
            var response = await customerLoanService.DefaultLoan(id);

            if (!response.IsSuccessful)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet]
        [Route("loan/{id}")]
        [ProducesResponseType(typeof(ResponseWrapper<CustomerLoan>), StatusCodes.Status200OK)] // Changed from string to CustomerLoan
        [ProducesResponseType(typeof(ResponseWrapper<CustomerLoan>), StatusCodes.Status400BadRequest)] // Changed from string to CustomerLoan
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<ActionResult> GetCustomerLoan(Guid id)
        {
            var response = await customerLoanService.GetLoanById(id);

            if (!response.IsSuccessful)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }


        [HttpGet]
        [Route("{customerId}/loan/pagenumber/{pagenum}/pagesize/{pagesize}")]
        [ProducesResponseType(typeof(ResponseWrapper<PagedResult<CustomerLoan>>), StatusCodes.Status200OK)] // Changed from string
        [ProducesResponseType(typeof(ResponseWrapper<PagedResult<CustomerLoan>>), StatusCodes.Status400BadRequest)] // Changed from string
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<ActionResult> GetCustomerLoans(Guid customerId, int pagenum, int pagesize,
                                                         [FromQuery] LoanStatusEnum? status,
                                                         [FromQuery] InterestFrequencyEnum? type)
        {
            var response = await customerLoanService.GetLoanByCustomerId(customerId, status, type, pagenum, pagesize);

            if (!response.IsSuccessful)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet]
        [Route("loan/pagenumber/{pagenum}/pagesize/{pagesize}")]
        [ProducesResponseType(typeof(ResponseWrapper<PagedResult<CustomerLoan>>), StatusCodes.Status200OK)] // Changed from string
        [ProducesResponseType(typeof(ResponseWrapper<PagedResult<CustomerLoan>>), StatusCodes.Status400BadRequest)] // Changed from string
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<ActionResult> GetLoans(int pagenum,
                                                 int pagesize,
                                                 [FromQuery] LoanStatusEnum? status,
                                                 [FromQuery] InterestFrequencyEnum? type)
        {
            var response = await customerLoanService.GetLoans(status, type, pagenum, pagesize);

            if (!response.IsSuccessful)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet]
        [Route("loan/{id}/paymentplan")]
        [ProducesResponseType(typeof(ResponseWrapper<List<LoanRepaymentPlanResponse>>), StatusCodes.Status200OK)] // Changed from string
        [ProducesResponseType(typeof(ResponseWrapper<List<LoanRepaymentPlanResponse>>), StatusCodes.Status400BadRequest)] // Changed from string
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<ActionResult> GetCustomerLoanPaymentPlan(Guid id)
        {
            var response = await customerLoanService.GetCustomerLoanRepaymentPlan(id);

            if (!response.IsSuccessful)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet]
        [Route("loan/paymentplan")]
        [ProducesResponseType(typeof(ResponseWrapper<List<LoanRepaymentPlanResponse>>), StatusCodes.Status200OK)] // Changed from string
        [ProducesResponseType(typeof(ResponseWrapper<List<LoanRepaymentPlanResponse>>), StatusCodes.Status400BadRequest)] // Changed from string
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<ActionResult> GetLoanPaymentPlans([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            var response = await customerLoanService.GetDueLoanRepaymentPlan(from, to);

            if (!response.IsSuccessful)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        // === NEW ENDPOINT: GetCustomerLoanSummaries ===
        /// <summary>
        /// Retrieves a paginated list of customer summaries, including their full name and all associated loan IDs.
        /// </summary>
        /// <param name="pageNumber">The current page number (default: 1).</param>
        /// <param name="pageSize">The number of items per page (default: 10).</param>
        /// <returns>A list of customer summaries with their loan IDs.</returns>
        [HttpGet("summaries")] // Endpoint: /api/Customer/summaries
        [ProducesResponseType(typeof(ResponseWrapper<PagedResult<CustomerLoanSummaryRes>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseWrapper<PagedResult<CustomerLoanSummaryRes>>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> GetCustomerLoanSummaries(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await customerService.GetCustomerLoanSummaries(pageNumber, pageSize);

            if (result.IsSuccessful)
            {
                return Ok(result);
            }
            return BadRequest(result); // Or other appropriate status code based on error
        }
        // === END NEW ENDPOINT ===
    }
}
