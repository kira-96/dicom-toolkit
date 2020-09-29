namespace SimpleDICOMToolkit.Validators
{
    using FluentValidation;
    using ViewModels;
    using static Helpers.ValidationHelper;

    public class EditDicomItemViewModelValidator : AbstractValidator<EditDicomItemViewModel>
    {
        public EditDicomItemViewModelValidator()
        {
            RuleFor(x => x.TagString).Must(BeAValidDicomTag).WithMessage("{PropertyName} must be a valid Dicom Tag.");
        }
    }
}
