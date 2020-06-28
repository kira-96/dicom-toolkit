using System.ComponentModel;

namespace SimpleDICOMToolkit.Client
{
    public enum FilmOrientation
    {
        /// <summary>
        /// 竖向
        /// </summary>
        [Description("Portrait")]
        Portrait,

        /// <summary>
        /// 横向
        /// </summary>
        [Description("Landscape")]
        Landscape,
    }

    /// <summary>
    /// Film size identification.
    /// </summary>
    /// <remarks>
    /// Defined Terms:
    /// <list type="bullet">
    /// <item><description>8INX10IN</description></item>
    /// <item><description>8_5INX11IN</description></item>
    /// <item><description>10INX12IN</description></item>
    /// <item><description>10INX14IN</description></item>
    /// <item><description>11INX14IN</description></item>
    /// <item><description>11INX17IN</description></item>
    /// <item><description>14INX14IN</description></item>
    /// <item><description>14INX17IN</description></item>
    /// <item><description>24CMX24CM</description></item>
    /// <item><description>24CMX30CM</description></item>
    /// <item><description>A4</description></item>
    /// <item><description>A3</description></item>
    /// </list>
    /// 
    /// Notes:
    /// 10INX14IN corresponds with 25.7CMX36.4CM
    /// A4 corresponds with 210 x 297 millimeters
    /// A3 corresponds with 297 x 420 millimeters
    /// </remarks>
    public enum FilmSize
    {
        /// <summary>
        /// 8英寸*10英寸
        /// 注意：Description值将作为printSCU中传递到dicom网络的参数值，禁止修改（包括大小写），后续enum值相同。
        ///      数字不可以开头，故开头加E标识（E -- Enum简称）
        /// </summary>
        [Description("8INX10IN")]
        E8InX10In,

        /// <summary>
        /// 8.5英寸*11英寸
        /// </summary>
        [Description("8_5INX11IN")]
        E8_5InX11In,

        /// <summary>
        /// 10英寸*12英寸
        /// </summary>
        [Description("10INX12IN")]
        E10InX12In,

        /// <summary>
        /// 10英寸*14英寸（实际大小：25.7cm*36.4cm）
        /// </summary>
        [Description("10INX14IN")]
        E10InX14In,

        /// <summary>
        /// 11英寸*14英寸
        /// </summary>
        [Description("11INX14IN")]
        E11InX14In,

        /// <summary>
        /// 11英寸*17英寸
        /// </summary>
        [Description("11INX17IN")]
        E11InX17In,

        /// <summary>
        /// 14英寸*14英寸
        /// </summary>
        [Description("14INX14IN")]
        E14InX14In,

        /// <summary>
        /// 14英寸*17英寸
        /// </summary>
        [Description("14INX17IN")]
        E14InX17In,

        /// <summary>
        /// 24厘米*24厘米
        /// </summary>
        [Description("24CMX24CM")]
        E24CmX24Cm,

        /// <summary>
        /// 24厘米*30厘米
        /// </summary>
        [Description("24CMX30CM")]
        E24CmX30Cm,

        /// <summary>
        /// A4纸(实际大小：210mm*297mm)
        /// </summary>
        [Description("A4")]
        A4,

        /// <summary>
        /// A3纸(实际大小：297mm*420mm)
        /// </summary>
        [Description("A3")]
        A3
    }

    /// <summary>
    /// Interpolation type by which the printer magnifies or decimates the image 
    /// in order to fit the image in the image box on film.
    /// </summary>
    /// <remarks>
    /// Defined Terms:
    /// <list type="bullet">
    /// <item><description>REPLICATE</description></item>
    /// <item><description>BILINEAR</description></item>
    /// <item><description>CUBIC</description></item>
    /// <item><description>NONE</description></item>
    /// </list>
    /// </remarks>
    public enum MagnificationType
    {
        /// <summary>
        /// REPLICATE
        /// </summary>
        [Description("Replicate")]
        Replicate,

        /// <summary>
        /// BILINEAR
        /// </summary>
        [Description("Bilinear")]
        Bilinear,

        /// <summary>
        /// CUBIC
        /// </summary>
        [Description("Cubic")]
        Cubic,

        /// <summary>
        /// NONE
        /// </summary>
        [Description("None")]
        None
    }

    /// <summary>
    /// Density of the film areas surrounding and between images on the film.
    /// </summary>
    /// <remarks>
    /// Defined Terms:
    /// <list type="bullet">
    /// <item><description>BLACK</description></item>
    /// <item><description>WHITE</description></item>
    /// <item>
    /// <description>
    /// i where i represents the desired density in hundredths of OD (e.g. 150 corresponds with 1.5 OD)
    /// </description>
    /// </item>
    /// </list>
    /// </remarks>
    public enum BorderDensity
    {
        /// <summary>
        /// BLACK
        /// </summary>
        [Description("Black")]
        Black,

        /// <summary>
        /// WHITE
        /// </summary>
        [Description("White")]
        White
    }

    /// <summary>
    /// Density of the image box area on the film that contains no image.
    /// </summary>
    /// <remarks>
    /// Defined Terms:
    /// <list type="bullet">
    /// <item><description>BLACK</description></item>
    /// <item><description>WHITE</description></item>
    /// <item><description>
    /// i where i represents the desired density in hundredths of OD 
    /// (e.g. 150 corresponds with 1.5 OD)
    /// </description></item>
    /// </list>
    /// </remarks>
    public enum EmptyImageDensity
    {
        /// <summary>
        /// BLACK
        /// </summary>
        [Description("Black")]
        Black,

        /// <summary>
        /// WHITE
        /// </summary>
        [Description("White")]
        White
    }

    /// <summary>
    /// Specifies whether a trim box shall be printed surrounding each image on the film.
    /// </summary>
    /// <remarks>
    /// Enumerated Values:
    /// <list type="bullet">
    /// <item><description>YES</description></item>
    /// <item><description>NO</description></item>
    /// </list>
    /// </remarks>
    public enum PrintTrim
    {
        /// <summary>
        /// YES
        /// </summary>
        [Description("Yes")]
        Yes,

        /// <summary>
        /// NO
        /// </summary>
        [Description("No")]
        No
    }

    /// <summary>
    /// Type of medium on which the print job will be printed.
    /// </summary>
    /// <remarks>
    /// Defined Terms:
    /// <list type="bullet">
    /// <item><description>PAPER</description></item>
    /// <item><description>CLEAR FILM</description></item>
    /// <item><description>BLUE FILM</description></item>
    /// <item><description>MAMMO CLEAR FILM</description></item>
    /// <item><description>MAMMO BLUE FILM</description></item>
    /// </list>
    /// </remarks>
    public enum MediumType
    {
        /// <summary>
        /// PAPER
        /// </summary>
        [Description("PaperMedium")]
        Paper,

        /// <summary>
        /// CLEAR FILM
        /// </summary>
        [Description("ClearFilm")]
        ClearFilm,

        /// <summary>
        /// BLUE FILM
        /// </summary>
        [Description("BlueFilm")]
        BlueFilm,

        /// <summary>
        /// MAMMO CLEAR FILM
        /// </summary>
        [Description("MammoClearFilm")]
        MammoClearFilm,

        /// <summary>
        /// MAMMO BLUE FILM
        /// </summary>
        [Description("MammoBlueFilm")]
        MammoBlueFilm
    }


    /// <summary>
    /// Film destination.
    /// </summary>
    /// <remarks>
    /// Defined Terms:
    /// <list type="bullet">
    /// <item>
    ///   <term>MAGAZINE</term>
    ///   <description>the exposed film is stored in film magazine</description>
    /// </item>
    /// <item>
    ///   <term>PROCESSOR</term>
    ///   <description>the exposed film is developed in film processor</description>
    /// </item>
    /// <item>
    ///   <term>BIN_i</term>
    ///   <description>
    ///   The exposed film is deposited in a sorter bin where “I” represents the bin 
    ///   number. Film sorter BINs shall be numbered sequentially starting from one and no maxium 
    ///   is placed on the number of BINs. The encoding of the BIN number shall not contain leading
    ///   zeros.
    ///   </description>
    /// </item>
    /// </list>
    /// </remarks>
    public class FilmDestination
    {
        public const string MAGAZINE = "MAGAZINE";

        public const string PROCESSOR = "PROCESSOR";
    }

    /// <summary>
    /// Specifies the priority of the print job.
    /// </summary>
    /// <remarks>
    /// Enumerated values:
    /// <list type="bullet">
    /// <item><description>HIGH</description></item>
    /// <item><description>MED</description></item>
    /// <item><description>LOW</description></item>
    /// </list>
    /// </remarks>
    public enum PrintPriority
    {
        /// <summary>
        /// HIGH
        /// </summary>
        [Description("Highest")]
        High,

        /// <summary>
        /// MED
        /// </summary>
        [Description("Medium")]
        Medium,

        /// <summary>
        /// LOW
        /// </summary>
        [Description("Lowest")]
        Low
    }

    public enum PrintColorType
    {
        [Description("GrayScale")]
        GrayScale,

        [Description("Color")]
        Color
    };

    public static class PrintEnumsEx
    {
        public static string ToStringEx(this FilmOrientation self)
        {
            switch (self)
            {
                case FilmOrientation.Portrait:
                    return "PORTRAIT";
                case FilmOrientation.Landscape:
                    return "LANDSCAPE";
                default:
                    return "PORTRAIT";
            }
        }

        public static string ToStringEx(this FilmSize self)
        {
            switch (self)
            {
                case FilmSize.E8InX10In:
                    return "8INX10IN";
                case FilmSize.E8_5InX11In:
                    return "8_5INX11IN";
                case FilmSize.E10InX12In:
                    return "10INX12IN";
                case FilmSize.E10InX14In:
                    return "10INX14IN";
                case FilmSize.E11InX14In:
                    return "11INX14IN";
                case FilmSize.E11InX17In:
                    return "11INX17IN";
                case FilmSize.E14InX14In:
                    return "14INX14IN";
                case FilmSize.E14InX17In:
                    return "14INX17IN";
                case FilmSize.E24CmX24Cm:
                    return "24CMX24CM";
                case FilmSize.E24CmX30Cm:
                    return "24CMX30CM";
                case FilmSize.A4:
                    return "A4";
                case FilmSize.A3:
                    return "A3";
                default:
                    return "A4";
            }
        }

        public static string ToStringEx(this MagnificationType self)
        {
            switch (self)
            {
                case MagnificationType.Replicate:
                    return "REPLICATE";
                case MagnificationType.Bilinear:
                    return "BILINEAR";
                case MagnificationType.Cubic:
                    return "CUBIC";
                case MagnificationType.None:
                    return "NONE";
                default:
                    return "NONE";
            }
        }

        public static string ToStringEx(this BorderDensity self)
        {
            switch (self)
            {
                case BorderDensity.Black:
                    return "BLACK";
                case BorderDensity.White:
                    return "WHITE";
                default:
                    return "BLACK";
            }
        }

        public static string ToStringEx(this EmptyImageDensity self)
        {
            switch (self)
            {
                case EmptyImageDensity.Black:
                    return "BLACK";
                case EmptyImageDensity.White:
                    return "WHITE";
                default:
                    return "BLACK";
            }
        }

        public static string ToStringEx(this PrintTrim self)
        {
            switch (self)
            {
                case PrintTrim.Yes:
                    return "YES";
                case PrintTrim.No:
                    return "NO";
                default:
                    return "";
            }
        }

        public static string ToStringEx(this MediumType self)
        {
            switch (self)
            {
                case MediumType.Paper:
                    return "PAPER";
                case MediumType.ClearFilm:
                    return "CLEAR FILM";
                case MediumType.BlueFilm:
                    return "BLUE FILM";
                case MediumType.MammoClearFilm:
                    return "MAMMO CLEAR FILM";
                case MediumType.MammoBlueFilm:
                    return "MAMMO BLUE FILM";
                default:
                    return "PAPER";
            }
        }

        public static string ToStringEx(this PrintPriority self)
        {
            switch (self)
            {
                case PrintPriority.High:
                    return "HIGH";
                case PrintPriority.Medium:
                    return "MED";
                case PrintPriority.Low:
                    return "LOW";
                default:
                    return "MED";
            }
        }
    }

    public class PrintOptions
    {
        public string JobLabel { get; set; } = "Simple Print Job";

        public string ImageDisplayFormat { get; set; } = "STANDARD\\1,1";

        public FilmOrientation Orientation { get; set; } = FilmOrientation.Portrait;

        public FilmSize FilmSize { get; set; } = FilmSize.A4;

        public MagnificationType MagnificationType { get; set; } = MagnificationType.None;

        public BorderDensity BorderDensity { get; set; } = BorderDensity.Black;

        public EmptyImageDensity EmptyImageDensity { get; set; } = EmptyImageDensity.Black;

        public PrintTrim PrintTrim { get; set; } = PrintTrim.No;

        public MediumType MediumType { get; set; } = MediumType.Paper;

        public string FilmDestination  { get; set; } = "MAGAZINE";

        public PrintPriority Priority { get; set; } = PrintPriority.Medium;

        public PrintColorType ColorType { get; set; } = PrintColorType.GrayScale;

        public int PrintNumberOfCopies { get; set; } = 1;
    }
}
