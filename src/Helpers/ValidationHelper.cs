using System.Linq;
using System.Text.RegularExpressions;

namespace SimpleDICOMToolkit.Helpers
{
    public static class ValidationHelper
    {
        /// <summary>
        /// Validate IP Address
        /// </summary>
        /// <param name="ip">input</param>
        /// <returns></returns>
        public static bool BeAValidIpAddress(string ip)
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
            if (Regex.IsMatch(ip, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$"))
            {
                return true;
            }

            // 一条完美精确匹配各种url网址的正则表达式
            // https://blog.csdn.net/qq569699973/article/details/94636893
            if (Regex.IsMatch(ip, @"^([hH][tT]{2}[pP]://|[hH][tT]{2}[pP][sS]://|[wW]{3}.|[wW][aA][pP].|[fF][tT][pP].|[fF][iI][lL][eE].)[-A-Za-z0-9+&@#/%?=~_|!:,.;]+[-A-Za-z0-9+&@#/%=~_|]$"))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Validate Port Number
        /// </summary>
        /// <param name="port">input</param>
        /// <returns></returns>
        public static bool BeAValidPortNumber(string port)
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
        public static bool BeAValidAETitle(string aet)
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
        /// Validate Age String
        /// https://github.com/fo-dicom/fo-dicom/blob/development/DICOM/DicomValidation.cs#L49
        /// </summary>
        /// <param name="content">input</param>
        /// <returns></returns>
        public static bool BeAValidAgeString(string content)
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
        /// Validate Code String
        /// https://github.com/fo-dicom/fo-dicom/blob/development/DICOM/DicomValidation.cs#L65
        /// </summary>
        /// <param name="code">input</param>
        /// <returns></returns>
        public static bool BeAValidCS(string code)
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
        public static bool BeAValidDate(string date)
        {
            if (string.IsNullOrEmpty(date))
            {
                return false;
            }

            return Regex.IsMatch(date, @"^\d{4}[-/.]?\d{2}[-/.]?\d{2}$");
        }

        /// <summary>
        /// Validate Dicom Tag
        /// </summary>
        /// <param name="tag">input</param>
        /// <returns></returns>
        public static bool BeAValidDicomTag(string tag)
        {
            if (string.IsNullOrEmpty(tag))
            {
                return false;
            }

            return Regex.IsMatch(tag, @"^\([0-9a-fA-F]{4},[0-9a-fA-F]{4}(:.*)?\)$");
        }
    }
}
