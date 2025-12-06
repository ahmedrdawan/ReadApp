using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MyReadsApp.Core.Common
{
    public class Response
    {
        public bool IsSuccess { get; protected set; }
        public List<string> Errors { get; protected set; } = new();
        public string Error { get; protected set; } = string.Empty;
        public int StatusCode { get; set; } = 200;

        protected Response(bool isSuccess, string error, int statusCode)
        {
            IsSuccess = isSuccess;
            StatusCode = statusCode;
            Error = error;
            Errors.Add(error);

        }
        protected Response(bool isSuccess, string error, List<string> errors, int statusCode)
        {
            IsSuccess = isSuccess;
            StatusCode = statusCode;
            Errors = errors;
            Error = Errors?.FirstOrDefault() ?? string.Empty;
        }
        public static Response Sucess(int statusCode = 200) => new(true, default ,statusCode);
        public static Response Failure(List<string> errors, int statusCode = 400) => new(false, default, errors ,statusCode);
        public static Response Failure(string error, int statusCode = 400) => new(false, error ,statusCode);
    }

    public class Response<T> : Response
    {
        
        public T? Value { get; private set; }

        private Response(bool isSuccess, T? value, string error,int statusCode = 200)
            :base(isSuccess, error, statusCode)
        {
            Value = value;
        }
        private Response(bool isSuccess, T? value, string error, List<string> errors, int statusCode = 200)
            : base(isSuccess, error, errors, statusCode)
        {
            Value = value;
        }
        public static Response<T> Sucess(T? value, int statusCode = 200) => new(true, value, string.Empty, statusCode);
        public static Response<T> Failure(List<string> errors, int statusCode = 400) => new(false, default, string.Empty,errors,statusCode);
        public static Response<T> Failure(string error, int statusCode = 400) => new(false, default,error, statusCode);

    }
}
