using FluentValidation;
using System.ComponentModel.DataAnnotations;
using VetSuccess.Application.DTOs.Client;

namespace VetSuccess.Application.Validators;

public class ClientUpdateDtoValidator : AbstractValidator<ClientUpdateDto>
{
    public ClientUpdateDtoValidator()
    {
        RuleFor(x => x.FullName)
            .MaximumLength(511)
            .When(x => !string.IsNullOrWhiteSpace(x.FullName))
            .WithMessage("Full name must not exceed 511 characters");

        RuleForEach(x => x.Emails)
            .SetValidator(new EmailUpdateDtoValidator())
            .When(x => x.Emails != null);

        RuleForEach(x => x.Phones)
            .SetValidator(new PhoneUpdateDtoValidator())
            .When(x => x.Phones != null);

        RuleForEach(x => x.Patients)
            .SetValidator(new PatientUpdateDtoValidator())
            .When(x => x.Patients != null);
    }
}

public class EmailUpdateDtoValidator : AbstractValidator<EmailUpdateDto>
{
    public EmailUpdateDtoValidator()
    {
        RuleFor(x => x.EmailAddress)
            .Must(BeValidEmail)
            .When(x => !string.IsNullOrWhiteSpace(x.EmailAddress))
            .WithMessage("Invalid email format");

        RuleFor(x => x.EmailAddress)
            .MaximumLength(254)
            .When(x => !string.IsNullOrWhiteSpace(x.EmailAddress))
            .WithMessage("Email address must not exceed 254 characters");
    }

    private bool BeValidEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return true;

        return new EmailAddressAttribute().IsValid(email);
    }
}

public class PhoneUpdateDtoValidator : AbstractValidator<PhoneUpdateDto>
{
    public PhoneUpdateDtoValidator()
    {
        RuleFor(x => x.PhoneNumber)
            .Must(BeValidPhone)
            .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber))
            .WithMessage("Phone number must be exactly 10 digits");

        RuleFor(x => x.PhoneType)
            .MaximumLength(50)
            .When(x => !string.IsNullOrWhiteSpace(x.PhoneType))
            .WithMessage("Phone type must not exceed 50 characters");
    }

    private bool BeValidPhone(string? phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            return true;

        // Phone number must be exactly 10 digits
        return phone.Length == 10 && phone.All(char.IsDigit);
    }
}

public class PatientUpdateDtoValidator : AbstractValidator<PatientUpdateDto>
{
    public PatientUpdateDtoValidator()
    {
        RuleFor(x => x.Uuid)
            .NotEmpty()
            .WithMessage("Patient UUID is required");

        RuleFor(x => x.OutcomeOduId)
            .MaximumLength(255)
            .When(x => !string.IsNullOrWhiteSpace(x.OutcomeOduId))
            .WithMessage("Outcome ODU ID must not exceed 255 characters");
    }
}
