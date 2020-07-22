namespace SimpleDICOMToolkit.Validators
{
    using FluentValidation;
    using System.Linq;
    using System.Text.RegularExpressions;
    using ViewModels;

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

        private bool BeAValidIpAddress(string ip)
        {
            if (string.IsNullOrEmpty(ip))
            {
                return false;
            }

            // 本机IP
            if (ip == "localhost")
            {
                return true;
            }

            // 正则表达式30分钟入门教程
            // https://deerchao.cn/tutorials/regex/regex.htm
            if (Regex.IsMatch(ip, @"((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)"))
            {
                return true;
            }

            // 一条完美精确匹配各种url网址的正则表达式
            // https://blog.csdn.net/qq569699973/article/details/94636893
            if (Regex.IsMatch(ip, @"([hH][tT]{2}[pP]://|[hH][tT]{2}[pP][sS]://|[wW]{3}.|[wW][aA][pP].|[fF][tT][pP].|[fF][iI][lL][eE].)[-A-Za-z0-9+&@#/%?=~_|!:,.;]+[-A-Za-z0-9+&@#/%=~_|]"))
            {
                return true;
            }

            return false;
        }

        private bool BeAValidPortNumber(string port)
        {
            if (string.IsNullOrEmpty(port))
            {
                return false;
            }

            // 正则表达式验证IP和端口格式的正确性
            // https://blog.csdn.net/u014594922/article/details/53018351
            return Regex.IsMatch(port, @"^([0-9]|[1-9]\d{1,3}|[1-5]\d{4}|6[0-4]\d{3}|65[0-4]\d{2}|655[0-2]\d|6553[0-5])$");
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
    }
}
