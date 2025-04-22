using Loan_Backend.API.Attribute;
using Loan_Backend.Domain.Interface;
using Loan_Backend.SharedKernel;
using Loan_Backend.SharedKernel.Model.DTO;
using Loan_Backend.SharedKernel.Model.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Mime;
using System.Security.Claims;

namespace Loan_Backend.API.Controllers
{
    [Route("api/user")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IAdminService adminService;
        private readonly ICurrentUser currentUser;
        public UserController(IAdminService adminService, ICurrentUser currentUser)
        {
            this.adminService = adminService;
            this.currentUser = currentUser;
        }

        [HttpPost]
        [Route("admin")]
        [ProducesResponseType(typeof(ResponseWrapper<AdminDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseWrapper<AdminDto>), StatusCodes.Status400BadRequest)]
        [Consumes(MediaTypeNames.Application.Json)]
        [HasPermission("can_create_admin")]
        public async Task<ActionResult> CreateAdmin(CreateAdminRequest request)
        {
            var response = await adminService.CreateAdmin(request, Guid.Parse(currentUser.Id!));

            if (!response.IsSuccessful)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost]
        [Route("operator")]
        [ProducesResponseType(typeof(ResponseWrapper<AdminDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseWrapper<AdminDto>), StatusCodes.Status400BadRequest)]
        [Consumes(MediaTypeNames.Application.Json)]
        [HasPermission("can_create_operator")]
        public async Task<ActionResult> CreateOperator(CreateAdminRequest request)
        {
            var response = await adminService.CreateAdmin(request, Guid.Parse(currentUser.Id!), isOperator: true);

            if (!response.IsSuccessful)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost]
        [Route("reset-password")]
        [ProducesResponseType(typeof(ResponseWrapper<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseWrapper<string>), StatusCodes.Status400BadRequest)]
        [Consumes(MediaTypeNames.Application.Json)]
        [HasPermission("can_reset_password")]
        public async Task<ActionResult> ResetPassword(PasswordResetRequest request)
        {
            var response = await adminService.ChangePassword(request.Id, request.Password, request.ConfirmedPassword);

            if (!response.IsSuccessful)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost]
        [Route("change-password")]
        [ProducesResponseType(typeof(ResponseWrapper<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseWrapper<string>), StatusCodes.Status400BadRequest)]
        [Consumes(MediaTypeNames.Application.Json)]
        [HasPermission("can_reset_password")]
        public async Task<ActionResult> ChangePassword(PasswordChangeRequest request)
        {
            var response = await adminService.ChangePassword(Guid.Parse(currentUser.Id!), request.Password, request.ConfirmedPassword);

            if (!response.IsSuccessful)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost]
        [Route("deactivate")]
        [ProducesResponseType(typeof(ResponseWrapper<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseWrapper<string>), StatusCodes.Status400BadRequest)]
        [Consumes(MediaTypeNames.Application.Json)]
        [HasPermission("can_deactivate_account")]
        public async Task<ActionResult> DeactivateAccount(DeactivateAccountRequest request)
        {
            var response = await adminService.DeactivateAdmin(request.Id);

            if (!response.IsSuccessful)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost]
        [Route("activate")]
        [ProducesResponseType(typeof(ResponseWrapper<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseWrapper<string>), StatusCodes.Status400BadRequest)]
        [Consumes(MediaTypeNames.Application.Json)]
        [HasPermission("can_activate_account")]
        public async Task<ActionResult> ActivateAccount(ActivateAccountRequest request)
        {
            var response = await adminService.ActivateAdmin(request.Id);

            if (!response.IsSuccessful)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet]
        [Route("admin/pagenumber/{pagenumber}/pagesize/{pagesize}")]
        [ProducesResponseType(typeof(ResponseWrapper<PagedResult<AdminDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseWrapper<PagedResult<AdminDto>>), StatusCodes.Status400BadRequest)]
        [Consumes(MediaTypeNames.Application.Json)]
        [HasPermission("can_view_admin_list")]
        public async Task<ActionResult> GetAllAdmins(int pagenumber, int pagesize)
        {
            var response = await adminService.GetAllUsersWithSpecifiedRole(false, pagenumber, pagesize);
            if (!response.IsSuccessful)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet]
        [Route("operator/pagenumber/{pagenumber}/pagesize/{pagesize}")]
        [ProducesResponseType(typeof(ResponseWrapper<PagedResult<AdminDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseWrapper<PagedResult<AdminDto>>), StatusCodes.Status400BadRequest)]
        [Consumes(MediaTypeNames.Application.Json)]
        [HasPermission("can_view_operator_list")]
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
