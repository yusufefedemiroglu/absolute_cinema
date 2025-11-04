using Application.DTOs.Product;
using FluentValidation;

namespace Application.Validators.Product
{
    public class ProductUpdateValidator : AbstractValidator<ProductUpdateDto>
    {
        public ProductUpdateValidator()
        {
            RuleFor(p => p.Id)
                .NotEmpty()
                .WithMessage("Product ID is required for update.");

            RuleFor(p => p.Price)
                .GreaterThan(0)
                .When(p => p.Price.HasValue)
                .WithMessage("Price must be greater than zero.");

            RuleFor(p => p.Stock)
                .GreaterThanOrEqualTo(0)
                .When(p => p.Stock.HasValue)
                .WithMessage("Stock cannot be negative.");

            RuleFor(p => p.Name)
                .MaximumLength(100)
                .When(p => !string.IsNullOrEmpty(p.Name))
                .WithMessage("Product name cannot exceed 100 characters.");

            RuleFor(p => p.Description)
                .MaximumLength(500)
                .When(p => !string.IsNullOrEmpty(p.Description))
                .WithMessage("Description cannot exceed 500 characters.");

            RuleFor(p => p.ImageUrl)
                .Must(url => string.IsNullOrEmpty(url) || Uri.IsWellFormedUriString(url, UriKind.Absolute))
                .WithMessage("ImageUrl must be a valid URL if provided.");
        }
    }
}