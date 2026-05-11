namespace MyReadsApp.Core.Common
{
    public class Response
    {
        public bool IsSuccess { get; protected set; }
        public string Message { get; protected set; } = string.Empty;
        public int StatusCode { get; set; } = 200;

        protected Response(bool isSuccess, string message, int statusCode)
        {
            IsSuccess = isSuccess;
            StatusCode = statusCode;
            Message = message ?? string.Empty;

        }
        public static Response Success(int statusCode = 200, string message = "") => new(true, message ,statusCode);
        public static Response Failure(string message, int statusCode = 400) => new(false, message ,statusCode);
    }

    public class Response<T> : Response
    {
        
        public T? Value { get; private set; }

        private Response(bool isSuccess, T? value, string message,int statusCode = 200)
            :base(isSuccess, message, statusCode)
        {
            Value = value;
        }
        public static Response<T> Success(T? value, int statusCode = 200, string message = "") => new(true, value, message, statusCode);
        public static Response<T> Failure(string message, int statusCode = 400) => new(false, default, message, statusCode);

    }
}
