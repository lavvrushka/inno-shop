using FluentValidation;
using MediatR;
using UserManagement.Application.Common.Exeptions;

namespace UserManagement.Application.Common.Validation
{
    public class ValidationService<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators) : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators = validators;
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (!_validators.Any())
                return await next();
            var context = new ValidationContext<TRequest>(request);
            var errorsDictionary = _validators
                .Select(x => x.Validate(context))
                .SelectMany(x => x.Errors)
                .Where(x => x != null)
                .GroupBy(
                    x => x.PropertyName.Substring(x.PropertyName.IndexOf('.') + 1),
                    x => x.ErrorMessage, (propertyName, errorMessages) => new
                    {
                        Key = propertyName,
                        Values = errorMessages.Distinct().ToArray()
                    }
                )
                .ToDictionary(x => x.Key, x => x.Values);
            if (errorsDictionary.Any())
            {
                throw new ValidationUserManagementException(errorsDictionary);
            }
            return await next();
        }
    }
}
