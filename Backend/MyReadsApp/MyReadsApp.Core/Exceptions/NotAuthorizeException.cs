namespace MyReadsApp.Core.Exceptions
{
    public class NotAuthorizeException : Exception
    {
        public NotAuthorizeException(string message) : base(message) { }
    }
}
