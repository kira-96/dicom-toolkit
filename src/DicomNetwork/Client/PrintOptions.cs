namespace SimpleDICOMToolkit.Client
{
    public enum FilmOrientation
    {
        /// <summary>
        /// 竖向
        /// </summary>
        Portrail,

        /// <summary>
        /// 横向
        /// </summary>
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
        /// 8INX10IN
        /// </summary>
        E8InX10In,

        /// <summary>
        /// 8_5INX11IN
        /// </summary>
        E8_5InX11In,

        /// <summary>
        /// 10INX12IN
        /// </summary>
        E10InX12In,

        /// <summary>
        /// 10INX14IN
        /// </summary>
        E10InX14In,

        /// <summary>
        /// 11INX14IN
        /// </summary>
        E11InX14In,

        /// <summary>
        /// 11INX17IN
        /// </summary>
        E11InX17In,

        /// <summary>
        /// 14INX14IN
        /// </summary>
        E14InX14In,

        /// <summary>
        /// 14INX17IN
        /// </summary>
        E14InX17In,

        /// <summary>
        /// 24CMX24CM
        /// </summary>
        E24CmX24Cm,

        /// <summary>
        /// 24CMX30CM
        /// </summary>
        E24CmX30Cm,

        /// <summary>
        /// A4
        /// </summary>
        A4,

        /// <summary>
        /// A3
        /// </summary>
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
        Replicate,

        /// <summary>
        /// BILINEAR
        /// </summary>
        Bilinear,

        /// <summary>
        /// CUBIC
        /// </summary>
        Cubic,

        /// <summary>
        /// NONE
        /// </summary>
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
        Black,

        /// <summary>
        /// WHITE
        /// </summary>
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
        Black,

        /// <summary>
        /// WHITE
        /// </summary>
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
        Yes,

        /// <summary>
        /// NO
        /// </summary>
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
        Paper,

        /// <summary>
        /// CLEAR FILM
        /// </summary>
        ClearFilm,

        /// <summary>
        /// BLUE FILM
        /// </summary>
        BlueFilm,

        /// <summary>
        /// MAMMO CLEAR FILM
        /// </summary>
        MammoClearFilm,

        /// <summary>
        /// MAMMO BLUE FILM
        /// </summary>
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
        High,

        /// <summary>
        /// MED
        /// </summary>
        Medium,

        /// <summary>
        /// LOW
        /// </summary>
        Low
    }

    public enum PrintColorType
    {
        GrayScale,

        Color
    };

    public static class PrintEnumsEx
    {
        public static string ToStringEx(this FilmOrientation self)
        {
            switch (self)
            {
                case FilmOrientation.Portrail:
                    return "PROTRAIL";
                case FilmOrientation.Landscape:
                    return "LANDSCAPE";
                default:
                    return "PROTRAIL";
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
        public string JobLabel { get; set; } = "Lonwin";

        public string ImageDisplayFormat { get; set; } = "STANDARD\\1,1";

        public FilmOrientation Orientation { get; set; } = FilmOrientation.Portrail;

        public FilmSize FilmSize { get; set; } = FilmSize.A4;

        public MagnificationType MagnificationType { get; set; } = MagnificationType.None;

        public BorderDensity BorderDensity { get; set; } = BorderDensity.Black;

        public EmptyImageDensity EmptyImageDensity { get; set; } = EmptyImageDensity.Black;

        public PrintTrim PrintTrim { get; set; } = PrintTrim.No;

        public MediumType MediumType { get; set; } = MediumType.Paper;

        public string FilmDestination  { get; set; } = "MAGAZINE";

        public PrintPriority Priority { get; set; } = PrintPriority.Medium;

        public PrintColorType ColorType { get; set; } = PrintColorType.GrayScale;
    }
}
