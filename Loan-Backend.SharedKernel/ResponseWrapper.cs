using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Loan_Backend.SharedKernel
{
    public class ResponseWrapper
    {
        public string Message { get; set; } = null!;
        public bool IsSuccessful { get; set; }
        [JsonPropertyName("errors")]
        public List<string> Errors { get; set; } = new List<string>();
    }

    public class ResponseWrapper<T> : ResponseWrapper where T : class
    {
        public T ResponseObject { get; set; } = null!;
        public bool ResponseObjectExists => ResponseObject != null;

        public static ResponseWrapper<T> Success(T instance, string message = "Successful") => new ResponseWrapper<T>()
        {
            Message = message,
            IsSuccessful = true,
            ResponseObject = instance
        };

        public static ResponseWrapper<T> Error(string error, string message = "") => new ResponseWrapper<T>()
        {
            IsSuccessful = false,
            Errors = new List<string> { error },
            Message = string.IsNullOrEmpty(message) ? error : message
        };

        public static ResponseWrapper<T> NotFound(T instance = null, string message = "") => new ResponseWrapper<T>()
        {
            Message = message,
            IsSuccessful = false,
            ResponseObject = instance,
            Errors = new List<string> { message }
        };
    }

}
