using FluentValidation;
using System.Text.Json;
using FluentValidation.Results;

namespace UserManagement.API.Middlewares
{
    public class ValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly JsonSerializerOptions _serializerOptions;

        public ValidationMiddleware(RequestDelegate next)
        {
            _next = next;
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var endpoint = context.GetEndpoint();
            if (endpoint == null)
            {
                await _next(context);
                return;
            }

            var validationTarget = endpoint.Metadata.GetMetadata<ValidationAttribute>()?.TargetType;
            if (validationTarget != null)
            {
                if (!await TryValidateRequestBody(context, validationTarget))
                {
                    return;
                }
            }

            await _next(context);
        }

        private async Task<bool> TryValidateRequestBody(HttpContext context, Type validationTarget)
        {
            try
            {
                context.Request.EnableBuffering();

                var body = await JsonSerializer.DeserializeAsync(
                    context.Request.Body,
                    validationTarget,
                    _serializerOptions);

                if (body == null)
                {
                    await WriteErrorResponseAsync(context, StatusCodes.Status400BadRequest, "Request body is required and cannot be empty.");
                    return false;
                }

                context.Request.Body.Position = 0;

                var validator = GetValidator(context, validationTarget);
                if (validator != null)
                {
                    var validationResult = await ValidateAsync(validator, body);
                    if (!validationResult.IsValid)
                    {
                        var errorDetails = validationResult.Errors.Select(e => new
                        {
                            PropertyName = e.PropertyName,
                            ErrorMessage = e.ErrorMessage
                        });

                        await WriteErrorResponseAsync(context, StatusCodes.Status400BadRequest, "Validation failed.", errorDetails);
                        return false;
                    }
                }
            }
            catch (JsonException ex)
            {
                await WriteErrorResponseAsync(context, StatusCodes.Status400BadRequest, "Invalid JSON format.", ex.Message);
                return false;
            }
            catch (Exception ex)
            {
                await WriteErrorResponseAsync(context, StatusCodes.Status500InternalServerError, "An unexpected error occurred.", ex.Message);
                return false;
            }

            return true;
        }

        private async Task<ValidationResult> ValidateAsync(IValidator validator, object body)
        {
            var validateMethod = validator.GetType().GetMethod("ValidateAsync", new[] { body.GetType(), typeof(CancellationToken) });
            if (validateMethod == null)
            {
                throw new InvalidOperationException("ValidateAsync method not found on validator.");
            }

            var validationTask = (Task)validateMethod.Invoke(validator, new object[] { body, CancellationToken.None });
            await validationTask.ConfigureAwait(false);

            var validationResultProperty = validationTask.GetType().GetProperty("Result");
            if (validationResultProperty == null)
            {
                throw new InvalidOperationException("Result property not found on validation task.");
            }

            return (ValidationResult)validationResultProperty.GetValue(validationTask)!;
        }

        private IValidator? GetValidator(HttpContext context, Type validationTarget)
        {
            var validatorType = typeof(IValidator<>).MakeGenericType(validationTarget);
            return context.RequestServices.GetService(validatorType) as IValidator;
        }

        private async Task WriteErrorResponseAsync(HttpContext context, int statusCode, string message, object? details = null)
        {
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            var response = new
            {
                message,
                details
            };

            await context.Response.WriteAsJsonAsync(response, _serializerOptions);
        }
    }
}
