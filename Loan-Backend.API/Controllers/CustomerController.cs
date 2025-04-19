using Loan_Backend.Domain.Interface;
using Loan_Backend.SharedKernel;
using Loan_Backend.SharedKernel.Model.DTO;
using Loan_Backend.SharedKernel.Model.Request;
using Loan_Backend.SharedKernel.Model.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Loan_Backend.API.Controllers
{
    [Route("api/customer")]
    [ApiController]
    [Authorize]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService customerService;
        public CustomerController(ICustomerService customerService)
        {
            this.customerService = customerService;
        }

        [HttpPost]
        public async Task<ActionResult> CreateCustomer(CreateCustomerReq request)
        {
            var userId = User.Claims.FirstOrDefault(e => e.Type == ClaimTypes.NameIdentifier);
            if(userId == null)
            {
                return Unauthorized(ResponseWrapper<CreateCustomerRes>.Error("Request is unauthorized."));
            }

            var response = await customerService.CreateCustomer(request,userId.Value);

            if (!response.IsSuccessful)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost]
        [Route("search")]
        public async Task<ActionResult> GetCustomer(CustomerFilter request)
        {
            var response = await customerService.GetCustomerByFilter(request);

            if (!response.IsSuccessful)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult> GetCustomerById(Guid id)
        {
            var response = await customerService.GetCustomerById(id);

            if (!response.IsSuccessful)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}
