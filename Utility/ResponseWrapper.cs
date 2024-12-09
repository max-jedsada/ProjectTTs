using Newtonsoft.Json;
using System.Net;

namespace Project.Utility
{

    public static class ResponseWrapper
    {
        public static Wrapper<T> Success<T>(HttpStatusCode httpStatusCode = HttpStatusCode.OK,
                                            T? data = default) where T : class
        {
            return new Wrapper<T>
            {
                Code = ((int)httpStatusCode).ToString(),
                Message = "Success",
                Data = data
            };
        }

        public static Wrapper<string> Success(HttpStatusCode httpStatusCode = HttpStatusCode.OK)
        {
            return new Wrapper<string>
            {
                Code = ((int)httpStatusCode).ToString(),
                Message = "Success",
                Data = null
            };
        }
    }

    public class Wrapper<T> where T : class
    {
        [JsonProperty("code")]
        public string? Code { get; set; }

        [JsonProperty("message")]
        public string? Message { get; set; }

        [JsonProperty("data")]
        public T? Data { get; set; }

        [JsonProperty("stackTrace", NullValueHandling = NullValueHandling.Ignore)]
        public string? StackTrace { get; set; }
    }
}
