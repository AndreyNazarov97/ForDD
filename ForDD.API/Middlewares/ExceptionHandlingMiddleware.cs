
using ForDD.Domain.Enum;
using ForDD.Domain.Result;
using System.Net;
using ILogger = Serilog.ILogger;

namespace ForDD.API.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            _logger.Error(exception, exception.Message);

            var errorMessage = exception.Message;
            var response = exception switch
            {
                UnauthorizedAccessException _ => new BaseResult() { ErrorMessage = errorMessage, ErrorCode = (int)HttpStatusCode.Unauthorized },
                _ => new BaseResult() { ErrorMessage = "Internal server error", ErrorCode = (int)HttpStatusCode.InternalServerError }
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)response.ErrorCode;
            await context.Response.WriteAsJsonAsync(response);

        }
    }
}
