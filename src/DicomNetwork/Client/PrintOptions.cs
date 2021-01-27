using SimpleDICOMToolkit.Infrastructure;

namespace SimpleDICOMToolkit.Client
{
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

        public FilmDestination FilmDestination  { get; set; } = FilmDestination.MAGAZINE;

        public PrintPriority Priority { get; set; } = PrintPriority.Medium;

        public PrintColorType ColorType { get; set; } = PrintColorType.GrayScale;

        public int PrintNumberOfCopies { get; set; } = 1;
    }
}
