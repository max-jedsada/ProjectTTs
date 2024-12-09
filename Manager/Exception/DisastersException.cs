using System.Net;

namespace Project.Manager.Exception
{
    public class DisastersException : BaseCustomException
    {
        public DisastersException(string message, HttpStatusCode httpStatusCode) : base(message, "Disasters", httpStatusCode) { }

        public class NoContent : DisastersException
        {
            public NoContent() : base($"Disasters no content.", HttpStatusCode.NoContent) { }
            public NoContent(int id) : base($"Disasters ({id}) is no content.", HttpStatusCode.NoContent) { }
            public NoContent(string name) : base($"{name}", HttpStatusCode.NoContent) { }
        }

        public class NotFound : DisastersException
        {
            public NotFound() : base($"Disasters not found.", HttpStatusCode.NotFound) { }
            public NotFound(int id) : base($"Disasters ({id}) is not found.", HttpStatusCode.NotFound) { }
            public NotFound(string name) : base($"{name}", HttpStatusCode.NotFound) { }
        }

        public class BadRequest : DisastersException
        {
            public BadRequest() : base($"bad request.", HttpStatusCode.BadRequest) { }
            public BadRequest(string name) : base($"{name}", HttpStatusCode.BadRequest) { }
        }

    }
}
