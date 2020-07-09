using System;

namespace SimpleDICOMToolkit.Services
{
    public interface IWindowsIntegrationService
    {
        bool IsSystemUsesLightTheme { get; }
        bool IsWindowPrevalenceAccentColor { get; }

        event EventHandler SystemUsesLightThemeChanged;

        event EventHandler WindowPrevalenceAccentColorChanged;

        void StartMonitoringSystemUsesLightTheme();
        void StopMonitoringSystemUsesLightTheme();

        void StartMonitoringWindowPrevalenceAccentColor();

        void StopMonitoringWindowPrevalenceAccentColor();
    }
}
