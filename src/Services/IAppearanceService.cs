using System;

namespace SimpleDICOMToolkit.Services
{
    public interface IAppearanceService
    {
        bool IsSystemUsesLightTheme { get; }
        bool IsWindowPrevalenceAccentColor { get; }

        event EventHandler SystemUsesLightThemeChanged;

        event EventHandler WindowPrevalenceAccentColorChanged;

        event EventHandler AccentColorChanged;

        void StartMonitoringSystemUsesLightTheme();
        void StopMonitoringSystemUsesLightTheme();

        void WatchWindowsColor(object window);

        void StartMonitoringWindowPrevalenceAccentColor();

        void StopMonitoringWindowPrevalenceAccentColor();

        void ApplyAccentColor();
    }
}
