using System.Net;

namespace Project.Provider.Exception
{
    public class TimeSettingException : BaseCustomException
    {
        public TimeSettingException(string message, HttpStatusCode httpStatusCode) : base(message, "setting", httpStatusCode) { }

        public class NotFound : TimeSettingException
        {
            public NotFound() : base($"setting not found.", HttpStatusCode.NotFound) { }
            public NotFound(int id = 0) : base($"setting id ({id}) is not found.", HttpStatusCode.NotFound) { }
        }

        public class NoContent : TimeSettingException
        {
            public NoContent() : base($"setting no content.", HttpStatusCode.NoContent) { }
            public NoContent(int id = 0) : base($"setting id ({id}) is no content.", HttpStatusCode.NoContent) { }
        }

    }
}
