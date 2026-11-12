
using CustomerManagement.Core.DTOs;
using FluentValidation;

namespace CustomerManagement.Core.Validators
{
    public class CustomerUpdateDtoValidator : AbstractValidator<CustomerUpdateDto>
    {
        public CustomerUpdateDtoValidator()
        {
            RuleFor(x => x.CustomerName)
                .NotEmpty().WithMessage("Customer name is required.")
                .MaximumLength(100).WithMessage("Customer name cannot exceed 100 characters.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Enter a valid email address.")
                .MaximumLength(150).WithMessage("Email cannot exceed 150 characters.");

            RuleFor(x => x.Mobile)
                .NotEmpty().WithMessage("Mobile number is required.")
                .Matches(@"^[0-9]{10}$")
                .WithMessage("Mobile number must contain exactly 10 digits.");

            RuleFor(x => x.City)
                .NotEmpty().WithMessage("City is required.")
                .MaximumLength(50).WithMessage("City cannot exceed 50 characters.");
        }
    }
}
