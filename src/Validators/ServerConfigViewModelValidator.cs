namespace SimpleDICOMToolkit.Validators
{
    using FluentValidation;
    using ViewModels;
    using static Helpers.ValidationHelper;

    public class ServerConfigViewModelValidator : AbstractValidator<ServerConfigViewModel>
    {
        public ServerConfigViewModelValidator()
        {
            RuleFor(x => x.ServerIP).Must(BeAValidIpAddress).WithMessage("{PropertyName} must be a valid IP address or web address.");
            RuleFor(x => x.ServerPort).Must(BeAValidPortNumber).WithMessage("{PropertyName} must be a valid port number.");
            RuleFor(x => x.ServerAET).Must(BeAValidAETitle).WithMessage("{PropertyName} must be a valid AE Title.");
            RuleFor(x => x.LocalAET).Must(BeAValidAETitle).WithMessage("{PropertyName} must be a valid AE Title.");
            RuleFor(x => x.Modality).NotNull().Must(BeAValidCS);
        }
    }
}
