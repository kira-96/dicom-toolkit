namespace SimpleDICOMToolkit.Validators
{
    using FluentValidation;
    using ViewModels;

    public class ServerConfigViewModelValidator : AbstractValidator<ServerConfigViewModel>
    {
        public ServerConfigViewModelValidator()
        {
            RuleFor(x => x.ServerIP).NotEmpty();  // maybe an IP address, or a web address
            RuleFor(x => x.ServerPort).NotEmpty().Matches(@"^\d{3,5}$").WithMessage("{PropertyName} must a number.");
            RuleFor(x => x.ServerAET).NotEmpty();
            RuleFor(x => x.LocalAET).NotEmpty();
            RuleFor(x => x.Modality).NotNull();
        }
    }
}
