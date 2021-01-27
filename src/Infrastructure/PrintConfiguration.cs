namespace SimpleDICOMToolkit.Infrastructure
{
    public class PrintConfiguration
    {
        public FilmOrientation Orientation { get; set; } = FilmOrientation.Portrait;

        public FilmSize Size { get; set; } = FilmSize.E14InX17In;

        public MagnificationType Magnification { get; set; } = MagnificationType.Cubic;

        public MediumType Medium { get; set; } = MediumType.BlueFilm;
    }
}
