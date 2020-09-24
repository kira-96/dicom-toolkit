namespace SimpleDICOMToolkit.Validators
{
    using FluentValidation;
    using ViewModels;
    using static Helpers.ValidationHelper;

    public class RegistrationViewModelValidator : AbstractValidator<RegistrationViewModel>
    {
        public RegistrationViewModelValidator()
        {
            RuleFor(x => x.PatientName).NotEmpty().WithMessage("{PropertyName} is required.");
            RuleFor(x => x.ScheduledAET).Must(BeAValidAETitle).WithMessage("{PropertyName} must be a valid AE Title.");
            RuleFor(x => x.Age).Must(BeAValidAgeString).WithMessage("{PropertyName} must be a valid Age String.");
            RuleFor(x => x.BirthDate).Must(BeAValidDate).WithMessage("{PropertyName} must be a valid Date Format.");
            RuleFor(x => x.ScheduledDate).Must(BeAValidDate).WithMessage("{PropertyName} must be a valid Date Format.");
            RuleFor(x => x.Modality).NotNull().Must(BeAValidCS);
        }
    }
}
