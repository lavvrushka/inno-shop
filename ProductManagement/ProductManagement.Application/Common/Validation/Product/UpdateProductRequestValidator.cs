using FluentValidation;
using ProductManagement.Application.DTOs.Product.Requests;

namespace ProductManagement.Application.Common.Validation.Product
{
    public class UpdateProductRequestValidator : AbstractValidator<UpdateProductRequest>
    {
        public UpdateProductRequestValidator()
        {
            RuleFor(e => e.Name)
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

            RuleFor(e => e.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");

        }
    }
}
