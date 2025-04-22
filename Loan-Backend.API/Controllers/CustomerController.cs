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
        [ProducesResponseType(typeof(ResponseWrapper<CreateCustomerRes>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseWrapper<CreateCustomerRes>), StatusCodes.Status400BadRequest)]
        [Consumes(MediaTypeNames.Application.Json)]
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
        [ProducesResponseType(typeof(ResponseWrapper<PagedResult<Customer>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseWrapper<PagedResult<Customer>>), StatusCodes.Status400BadRequest)]
        [Consumes(MediaTypeNames.Application.Json)]
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
    }
}
