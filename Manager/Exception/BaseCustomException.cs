using System.Net;

namespace Project.Manager.Exception
{
    public abstract class BaseCustomException : System.Exception
    {
        public HttpStatusCode statusCode { get; set; }
        public string title { get; }
        public string Code { get => $"{title}:{(int)statusCode}"; }

        public BaseCustomException(string message, string prefix, HttpStatusCode httpStatusCode = HttpStatusCode.InternalServerError) : base(message)
        {
            title = prefix;
            statusCode = httpStatusCode;
        }
    }
}
