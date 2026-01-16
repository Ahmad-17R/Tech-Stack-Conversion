using FluentValidation;
using System.ComponentModel.DataAnnotations;
using VetSuccess.Application.DTOs.Practice;

namespace VetSuccess.Application.Validators;

public class PracticeSettingsDtoValidator : AbstractValidator<PracticeSettingsDto>
{
    public PracticeSettingsDtoValidator()
    {
        // Date range validation - start date must come before end date
        RuleFor(x => x)
            .Must(x => ValidateDateRange(x.StartDateForLaunch, x.EndDateForLaunch))
            .When(x => x.StartDateForLaunch.HasValue && x.EndDateForLaunch.HasValue)
            .WithMessage("Start date for launch must be before end date for launch");

        // Email validation
        RuleFor(x => x.Email)
            .Must(BeValidEmail)
            .When(x => !string.IsNullOrWhiteSpace(x.Email))
            .WithMessage("Invalid email format");

        RuleFor(x => x.SchedulerEmail)
            .Must(BeValidEmail)
            .When(x => !string.IsNullOrWhiteSpace(x.SchedulerEmail))
            .WithMessage("Invalid scheduler email format");

        RuleFor(x => x.RdoEmail)
            .Must(BeValidEmail)
            .When(x => !string.IsNullOrWhiteSpace(x.RdoEmail))
            .WithMessage("Invalid RDO email format");

        // Phone validation
        RuleFor(x => x.SmsPhone)
            .Must(BeValidPhone)
            .When(x => !string.IsNullOrWhiteSpace(x.SmsPhone))
            .WithMessage("SMS phone must be exactly 10 digits");

        RuleFor(x => x.SmsSendersPhone)
            .Must(BeValidPhone)
            .When(x => !string.IsNullOrWhiteSpace(x.SmsSendersPhone))
            .WithMessage("SMS sender's phone must be exactly 10 digits");

        // Conditional validation - if SMS is enabled, certain fields are required
        RuleFor(x => x.SmsPracticeName)
            .NotEmpty()
            .When(x => x.EnableSmsReminders == true)
            .WithMessage("SMS practice name is required when SMS reminders are enabled");

        RuleFor(x => x.SmsPhone)
            .NotEmpty()
            .When(x => x.EnableSmsReminders == true)
            .WithMessage("SMS phone is required when SMS reminders are enabled");

        // Conditional validation - if email updates are enabled, email is required
        RuleFor(x => x.Email)
            .NotEmpty()
            .When(x => x.EnableEmailReminders == true)
            .WithMessage("Email is required when email reminders are enabled");

        // Days validation
        RuleFor(x => x.DaysBeforeAppointment)
            .GreaterThanOrEqualTo(0)
            .When(x => x.DaysBeforeAppointment.HasValue)
            .WithMessage("Days before appointment must be greater than or equal to 0");

        RuleFor(x => x.DaysAfterAppointment)
            .GreaterThanOrEqualTo(0)
            .When(x => x.DaysAfterAppointment.HasValue)
            .WithMessage("Days after appointment must be greater than or equal to 0");
    }

    private bool BeValidEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return true;

        return new EmailAddressAttribute().IsValid(email);
    }

    private bool BeValidPhone(string? phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            return true;

        // Phone number must be exactly 10 digits
        return phone.Length == 10 && phone.All(char.IsDigit);
    }

    private bool ValidateDateRange(DateTime? startDate, DateTime? endDate)
    {
        if (!startDate.HasValue || !endDate.HasValue)
            return true;

        return startDate.Value < endDate.Value;
    }
}
