using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Loan_Backend.SharedKernel
{
    public class ExceptionHandler
    {
        private readonly RequestDelegate _next;

        public ExceptionHandler(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }catch(Exception ex)
            {
                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                var response = ResponseWrapper<string>.Error(ex.Message, $"An unexpected error occurred. Please try again | Stack Trace: {ex.StackTrace}");

                httpContext.Response.ContentType = "application/json";
                await httpContext.Response.WriteAsync(response.ToJson());
            }
        }
    }
}
