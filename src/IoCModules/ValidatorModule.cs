namespace SimpleDICOMToolkit.IoCModules
{
    using FluentValidation;
    using Stylet;
    using StyletIoC;
    using Validators;

    internal class ValidatorModule : StyletIoCModule
    {
        protected override void Load()
        {
            Bind(typeof(IModelValidator<>)).To(typeof(FluentModelValidator<>));
            Bind(typeof(IValidator<>)).ToAllImplementations();
        }
    }
}
