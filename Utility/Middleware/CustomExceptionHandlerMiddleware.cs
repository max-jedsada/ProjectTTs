using Project.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;
using Project.Provider.Exception;
using Microsoft.Data.SqlClient;

namespace Project.API
{
    public class CustomExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IHostEnvironment _env;
        private readonly ILogger<CustomExceptionHandlerMiddleware> _logger;

        public CustomExceptionHandlerMiddleware(RequestDelegate next,
                                                IHostEnvironment env,
                                                ILogger<CustomExceptionHandlerMiddleware> logger)
        {
            _next = next;
            _env = env;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
            }
            catch (System.Exception ex)
            {
                await HandleException(context, ex);
            }
        }

        private async Task HandleException(HttpContext context, System.Exception exception)
        {
            var response = context.Response;
            var isProduction = _env.IsProduction();
            response.ContentType = "application/json";

            if (exception is BaseCustomException baseException)
            {
                response.StatusCode = (int)baseException.StatusCode;

                var wrapError = new Wrapper<string>
                {
                    Code = baseException.Code,
                    Message = baseException.Message
                };

                _logger.LogError(exception, "{message}", exception.Message);

                await response.WriteAsync(JsonConvert.SerializeObject(wrapError));
            }
            else if (exception.InnerException is not null
                     && exception.InnerException is SqlException sqlException
                     && sqlException.Number == 547)
            {
                response.StatusCode = StatusCodes.Status409Conflict;

                var message = "Current data has been used in other table.";

                var wrapError = new Wrapper<string>
                {
                    Code = ((int)HttpStatusCode.Conflict).ToString(),
                    Message = message,
                    StackTrace = !isProduction ? exception.StackTrace : null,
                };

                _logger.LogError(exception, "{message}", exception.Message);

                await response.WriteAsync(JsonConvert.SerializeObject(wrapError));
            }
            else
            {

                response.StatusCode = 500;

                var errorResponse = new Wrapper<object>
                {
                    Code = "500",
                    Message = exception.Message,
                    Data = exception.Data,
                    StackTrace = !isProduction ? exception.StackTrace : null
                };

                _logger.LogError(exception, "{message}", exception.Message);

                await response.WriteAsync(JsonConvert.SerializeObject(errorResponse));
            }
        }

    }
}
