using FluentValidation;
using System.Text.Json;

namespace ProductManagement.API.Middlewares
{
    public class ValidationMiddleware : IMiddleware
    {
        private readonly IServiceProvider _serviceProvider;

        public ValidationMiddleware(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (context.Request.Method == HttpMethods.Post || context.Request.Method == HttpMethods.Put)
            {
                context.Request.EnableBuffering();

                var body = await ReadRequestBodyAsync(context.Request);

                if (string.IsNullOrWhiteSpace(body))
                {
                    await next(context);
                    return;
                }

                var endpoint = context.GetEndpoint();
                var actionDescriptor = endpoint?.Metadata?.GetMetadata<Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor>();
                var parameterType = actionDescriptor?.Parameters.FirstOrDefault()?.ParameterType;

                if (parameterType == null)
                {
                    await next(context);
                    return;
                }

                var request = JsonSerializer.Deserialize(body, parameterType, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (request == null)
                {
                    await next(context);
                    return;
                }

                var validatorType = typeof(IValidator<>).MakeGenericType(parameterType);
                var validator = _serviceProvider.GetService(validatorType) as IValidator;

                if (validator != null)
                {
                    var validationResult = await validator.ValidateAsync(new ValidationContext<object>(request));

                    if (!validationResult.IsValid)
                    {
                        context.Response.StatusCode = StatusCodes.Status400BadRequest;
                        await context.Response.WriteAsJsonAsync(new
                        {
                            Errors = validationResult.Errors.Select(e => new { PropertyName = e.PropertyName, ErrorMessage = e.ErrorMessage })
                        });
                        return;
                    }
                }
            }

            await next(context);
        }

        private async Task<string> ReadRequestBodyAsync(HttpRequest request)
        {
            request.Body.Position = 0;
            using var reader = new StreamReader(request.Body, leaveOpen: true);
            var body = await reader.ReadToEndAsync();
            request.Body.Position = 0;
            return body;
        }
    }
}
