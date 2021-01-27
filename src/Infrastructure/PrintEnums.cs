using System.ComponentModel;

namespace SimpleDICOMToolkit.Infrastructure
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
    public enum FilmDestination
    {
        [Description("MAGAZINE")]
        MAGAZINE,

        [Description("PROCESSOR")]
        PROCESSOR,

        [Description("NOT SUPPORTED")]
        BIN,
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
            return self switch
            {
                FilmOrientation.Portrait => "PORTRAIT",
                FilmOrientation.Landscape => "LANDSCAPE",
                _ => "PORTRAIT",
            };
        }

        public static string ToStringEx(this FilmSize self)
        {
            return self switch
            {
                FilmSize.E8InX10In => "8INX10IN",
                FilmSize.E8_5InX11In => "8_5INX11IN",
                FilmSize.E10InX12In => "10INX12IN",
                FilmSize.E10InX14In => "10INX14IN",
                FilmSize.E11InX14In => "11INX14IN",
                FilmSize.E11InX17In => "11INX17IN",
                FilmSize.E14InX14In => "14INX14IN",
                FilmSize.E14InX17In => "14INX17IN",
                FilmSize.E24CmX24Cm => "24CMX24CM",
                FilmSize.E24CmX30Cm => "24CMX30CM",
                FilmSize.A4 => "A4",
                FilmSize.A3 => "A3",
                _ => "A4",
            };
        }

        public static string ToStringEx(this MagnificationType self)
        {
            return self switch
            {
                MagnificationType.Replicate => "REPLICATE",
                MagnificationType.Bilinear => "BILINEAR",
                MagnificationType.Cubic => "CUBIC",
                MagnificationType.None => "NONE",
                _ => "NONE",
            };
        }

        public static string ToStringEx(this BorderDensity self)
        {
            return self switch
            {
                BorderDensity.Black => "BLACK",
                BorderDensity.White => "WHITE",
                _ => "BLACK",
            };
        }

        public static string ToStringEx(this EmptyImageDensity self)
        {
            return self switch
            {
                EmptyImageDensity.Black => "BLACK",
                EmptyImageDensity.White => "WHITE",
                _ => "BLACK",
            };
        }

        public static string ToStringEx(this PrintTrim self)
        {
            return self switch
            {
                PrintTrim.Yes => "YES",
                PrintTrim.No => "NO",
                _ => "",
            };
        }

        public static string ToStringEx(this MediumType self)
        {
            return self switch
            {
                MediumType.Paper => "PAPER",
                MediumType.ClearFilm => "CLEAR FILM",
                MediumType.BlueFilm => "BLUE FILM",
                MediumType.MammoClearFilm => "MAMMO CLEAR FILM",
                MediumType.MammoBlueFilm => "MAMMO BLUE FILM",
                _ => "PAPER",
            };
        }

        public static string ToStringEx(this FilmDestination self)
        {
            return self switch
            {
                FilmDestination.MAGAZINE => "MAGAZINE",
                FilmDestination.PROCESSOR => "PROCESSOR",
                _ => "MAGAZINE",
            };
        }

        public static string ToStringEx(this PrintPriority self)
        {
            return self switch
            {
                PrintPriority.High => "HIGH",
                PrintPriority.Medium => "MED",
                PrintPriority.Low => "LOW",
                _ => "MED",
            };
        }
    }
}
