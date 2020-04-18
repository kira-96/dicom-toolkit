using Nett;
using Stylet;
using System;
using System.Collections.Generic;

namespace Config.ViewModels
{
    public class PrintOptionsViewModel : Screen, IConfigViewModel
    {
        public List<string> Orientations { get; } = new List<string>()
        {
            "PROTRAIL",
            "LANDSCAPE"
        };

        public List<string> FilmSize { get; } = new List<string>()
        {
            "8in×10in",
            "8.5in×11in",
            "10in×12in",
            "10in×14in",
            "11in×14in",
            "11in×17in",
            "14in×14in",
            "14in×17in",
            "24cm×24cm",
            "24cm×30cm",
            "A4",
            "A3"
        };

        public List<string> MagnificationTypes { get; } = new List<string>()
        {
            "REPLICATE",
            "BILINEAR",
            "CUBIC",
            "NONE"
        };

        public List<string> MediumTypes { get; } = new List<string>()
        {
            "PAPER",
            "CLEAR FILM",
            "BLUE FILM",
            "MAMMO CLEAR FILM",
            "MAMMO BLUE FILM"
        };

        private int _orientation = 0;
        private int _size = 7;
        private int _magnification = 3;
        private int _medium = 2;

        public int Orientation
        {
            get => _orientation;
            set => SetAndNotify(ref _orientation, value);
        }

        public int Size
        {
            get => _size;
            set => SetAndNotify(ref _size, value);
        }

        public int Magnification
        {
            get => _magnification;
            set => SetAndNotify(ref _magnification, value);
        }

        public int Medium
        {
            get => _medium;
            set => SetAndNotify(ref _medium, value);
        }

        public PrintOptionsViewModel()
        {
            DisplayName = "Print Options";
        }

        protected override void OnViewLoaded()
        {
            base.OnViewLoaded();

            LoadConfigs((Parent as ShellViewModel).ConfigTable);
        }

        private void LoadConfigs(TomlTable table)
        {
            if (table.ContainsKey("PrintOptions"))
            {
                TomlTable options = table.Get<TomlTable>("PrintOptions");
                if (options.ContainsKey("Orientation"))
                    Orientation = (int)options.Get<TomlInt>("Orientation").Value;
                if (options.ContainsKey("Size"))
                    Orientation = (int)options.Get<TomlInt>("Size").Value;
                if (options.ContainsKey("Magnification"))
                    Magnification = (int)options.Get<TomlInt>("Magnification").Value;
                if (options.ContainsKey("Medium"))
                    Magnification = (int)options.Get<TomlInt>("Medium").Value;
            }
        }

        public void SaveConfig()
        {
            TomlTable options = Toml.Create();
            options.Add("Orientation", Orientation);
            options.Add("Size", Size);
            options.Add("Magnification", Magnification);
            options.Add("Medium", Medium);

            TomlTable table = (Parent as ShellViewModel).ConfigTable;

            if (table.ContainsKey("PrintOptions"))
                table.Remove("PrintOptions");

            table.Add("PrintOptions", options);
        }

        public void Dispose()
        {
            // TODO
        }
    }
}
