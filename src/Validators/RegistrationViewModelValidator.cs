namespace SimpleDICOMToolkit.Validators
{
    using FluentValidation;
    using System.Linq;
    using System.Text.RegularExpressions;
    using ViewModels;

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

        /// <summary>
        /// Validate Age String
        /// https://github.com/fo-dicom/fo-dicom/blob/development/DICOM/DicomValidation.cs#L49
        /// </summary>
        /// <param name="content">input</param>
        /// <returns></returns>
        private bool BeAValidAgeString(string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                return true;
            }

            // 4 charachters fixed
            // one of the following formats -- nnnD, nnnW, nnnM, nnnY; where nnn shall contain the number of days for D, weeks for W, months for M, or years for Y.
            if (Regex.IsMatch(content, @"^\d\d\d[DWMY]$"))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Validate AE Title
        /// https://github.com/fo-dicom/fo-dicom/blob/development/DICOM/DicomValidation.cs#L29
        /// </summary>
        /// <param name="aet">input</param>
        /// <returns></returns>
        private bool BeAValidAETitle(string aet)
        {
            if (string.IsNullOrEmpty(aet))
            {
                return false;
            }

            if (aet.Length > 16)  // may not be longer than 16 characters
            {
                return false;
            }

            if (Regex.IsMatch(aet, @"^\s*$"))  // may not contain only of spaces
            {
                return false;
            }

            if (aet.Contains("\\") || aet.ToCharArray().Any(char.IsControl))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Validate Code String
        /// https://github.com/fo-dicom/fo-dicom/blob/development/DICOM/DicomValidation.cs#L65
        /// </summary>
        /// <param name="code">input</param>
        /// <returns></returns>
        private bool BeAValidCS(string code)
        {
            if (code.Length > 16)  // 16 bytes maximum
            {
                return false;
            }

            // Uppercase characters, "0" - "9", the SPACE character, and underscore "_", of the Default Character Repertoire
            return Regex.IsMatch(code, @"^[A-Z0-9_ ]*$");
        }

        /// <summary>
        /// Validate Date Time
        /// </summary>
        /// <param name="date">input</param>
        /// <returns></returns>
        private bool BeAValidDate(string date)
        {
            if (string.IsNullOrEmpty(date))
            {
                return false;
            }

            return Regex.IsMatch(date, @"^\d{4}[-/.]?\d{2}[-/.]?\d{2}$");
        }
    }
}
