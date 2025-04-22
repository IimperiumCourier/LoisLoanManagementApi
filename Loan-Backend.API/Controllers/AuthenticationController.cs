using Loan_Backend.API.Attribute;
using Loan_Backend.Domain.Interface;
using Loan_Backend.Infrastructure.Service;
using Loan_Backend.SharedKernel.Model.DTO;
using Loan_Backend.SharedKernel.Model.Request;
using Loan_Backend.SharedKernel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Net.Mime;
using Loan_Backend.SharedKernel.Model.Response;

namespace Loan_Backend.API.Controllers
{
    [Route("api/authentication")]
    [ApiController]
    [Authorize]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService authenticationService;
        public AuthenticationController(IAuthenticationService authenticationService)
        {
            this.authenticationService = authenticationService;   
        }

        [HttpPost]
        [Route("login")]
        [ProducesResponseType(typeof(ResponseWrapper<AuthenticationResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseWrapper<AuthenticationResponse>), StatusCodes.Status400BadRequest)]
        [Consumes(MediaTypeNames.Application.Json)]
        [AllowAnonymous]
        public async Task<ActionResult> Login(AuthenticationRequest request)
        {
            var response = await authenticationService.Authenticate(request);

            if (!response.IsSuccessful)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost]
        [Route("refresh-jwt-token")]
        [ProducesResponseType(typeof(ResponseWrapper<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseWrapper<string>), StatusCodes.Status400BadRequest)]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<ActionResult> RefreshJWTToken([FromQuery] string refreshToken)
        {
            var response = await authenticationService.RefreshToken(refreshToken);

            if (!response.IsSuccessful)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}
