namespace SimpleDICOMToolkit.Validators
{
    using FluentValidation;
    using ViewModels;
    using static Helpers.ValidationHelper;

    public class MoveToViewModelValidator : AbstractValidator<MoveToViewModel>
    {
        public MoveToViewModelValidator()
        {
            RuleFor(x => x.ServerIP).Must(BeAValidIpAddress).WithMessage("{PropertyName} must be a valid IP address or web address.");
            RuleFor(x => x.ServerPort).Must(BeAValidPortNumber).WithMessage("{PropertyName} must be a valid port number.");
            RuleFor(x => x.ServerAET).Must(BeAValidAETitle).WithMessage("{PropertyName} must be a valid AE Title.");
        }
    }
}
