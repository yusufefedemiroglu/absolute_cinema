using System.ComponentModel.DataAnnotations;
using Application.DTOs.Product;
using FluentValidation;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Application.Validators
{
    public class ProductCreateValidator : AbstractValidator<ProductCreateDto>
    {
        private readonly AppDbContext _context;

        public ProductCreateValidator(AppDbContext context)
        {
            _context = context;

            RuleFor(p => p.Name)
                .NotEmpty().WithMessage("Product name is required.")
                .MaximumLength(100).WithMessage("Product name cannot exceed 100 characters.");

            RuleFor(p => p.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.").NotEmpty().WithMessage("Description cannot be empty.");

            RuleFor(p => p.Price)
                .GreaterThan(0).WithMessage("Price must be greater than zero.").NotEmpty().WithMessage("Price cannot be empty.");

            RuleFor(p => p.Stock)
                .GreaterThanOrEqualTo(0).WithMessage("Stock cannot be negative.").NotEmpty().WithMessage("Stock cannot be empty.");

            RuleFor(p => p.ImageUrl)
                .Must(url => string.IsNullOrEmpty(url) || Uri.IsWellFormedUriString(url, UriKind.Absolute))
                .WithMessage("ImageUrl must be a valid URL if provided.");

        }
        private bool TitleExists(int titleId)
        {
            return _context.Titles.Any(t => t.Id == titleId);
        }
    }
}