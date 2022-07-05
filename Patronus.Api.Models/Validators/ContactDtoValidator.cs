using FluentValidation;

namespace Patronus.Api.Models.Validators
{
    public class ContactDtoValidator : AbstractValidator<ContactDto>
    {
        public ContactDtoValidator()
        {
            RuleFor(c => c.Name).NotEmpty().WithErrorCode("Name is required.");

            //source: https://stackoverflow.com/questions/16699007/regular-expression-to-match-standard-10-digit-phone-number
            RuleFor(c => c.Phone).Matches(@"^(\+\d{1,2}\s)?\(?\d{3}\)?[\s.-]?\d{3}[\s.-]?\d{4}$").When(c => !string.IsNullOrWhiteSpace(c.Phone)).WithMessage("Phone is invalid.");

            RuleFor(c => c.Email).EmailAddress().When(c => !string.IsNullOrWhiteSpace(c.Email)).WithMessage("Email is invalid.");

            RuleFor(c => c.State).Length(2).When(c => !string.IsNullOrWhiteSpace(c.State)).Matches(@"[a-zA-Z]").When(c => !string.IsNullOrWhiteSpace(c.State)).WithMessage("State must be 2 letters.");
        }

        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<ContactDto>.CreateWithOptions((ContactDto)model, x => x.IncludeProperties(propertyName)));
            if (result.IsValid)
                return Array.Empty<string>();
            return result.Errors.Select(e => e.ErrorMessage);
        };
    }
}
