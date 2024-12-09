using System.Net;

namespace Project.Provider.Exception
{
    public abstract class BaseCustomException : System.Exception
    {
        public HttpStatusCode StatusCode { get; set; }
        public string Prefix { get; }
        public string Code { get => $"{Prefix}{(int)StatusCode}"; }

        public BaseCustomException(string message, string prefix, HttpStatusCode statusCode = HttpStatusCode.InternalServerError) : base(message)
        {
            Prefix = prefix;
            StatusCode = statusCode;
        }
    }
}
