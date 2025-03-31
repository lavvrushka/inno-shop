using FluentValidation;
using System.Net;
using System.Text.Json;

namespace ProductManagement.API.Middlewares
{
    public class GlobalExceptionMiddleware : IMiddleware
    {
        private readonly ILogger<GlobalExceptionMiddleware> _logger;
        private readonly JsonSerializerOptions _serializerOptions;

        public GlobalExceptionMiddleware(ILogger<GlobalExceptionMiddleware> logger)
        {
            _logger = logger;
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
            };
        }
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                if (!context.Response.HasStarted)
                {
                    _logger.LogError(ex, "Необработанное исключение: {Message}", ex.Message);
                    await HandleExceptionAsync(context, ex);
                }
                else
                {
                    _logger.LogWarning("Ответ уже был начат, обработка исключения невозможна: {Message}", ex.Message);
                }
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var (statusCode, response) = GetErrorResponse(exception);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var responseBody = JsonSerializer.Serialize(response, _serializerOptions);
            await context.Response.WriteAsync(responseBody);
        }

        private static (HttpStatusCode statusCode, object response) GetErrorResponse(Exception exception) =>
            exception switch
            {
                ValidationException validationException => (
                    HttpStatusCode.BadRequest,
                    new
                    {
                        message = "Ошибка валидации.",
                        details = validationException.Errors.Select(e => new { e.PropertyName, e.ErrorMessage })
                    }),

                UnauthorizedAccessException => (
                    HttpStatusCode.Unauthorized,
                    new
                    {
                        message = "Доступ запрещен.",
                        details = (object?)null
                    }),

                InvalidOperationException invalidOperationException => (
                    HttpStatusCode.BadRequest,
                    new
                    {
                        message = invalidOperationException.Message,
                        details = (object?)null
                    }),

                KeyNotFoundException => (
                    HttpStatusCode.NotFound,
                    new
                    {
                        message = "Ресурс не найден.",
                        details = (object?)null
                    }),

                _ => (
                    HttpStatusCode.InternalServerError,
                    new
                    {
                        message = "Произошла непредвиденная ошибка.",
                        details = exception.Message
                    })
            };
    }
}
