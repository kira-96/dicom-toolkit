using System;

namespace SimpleDICOMToolkit.Services
{
    public interface IAppearanceService
    {
        /// <summary>
        /// 当前系统是否为亮色主题（Windows 10）
        /// </summary>
        bool IsSystemUsesLightTheme { get; }

        /// <summary>
        /// 窗口色是否跟随系统主题颜色（Windows 10）
        /// </summary>
        bool IsWindowPrevalenceAccentColor { get; }

        /// <summary>
        /// 系统亮暗主题变更事件（Windows 10）
        /// </summary>
        event EventHandler SystemUsesLightThemeChanged;

        /// <summary>
        /// 窗口色是否跟随系统主题颜色变更事件（Windows 10）
        /// </summary>
        event EventHandler WindowPrevalenceAccentColorChanged;

        /// <summary>
        /// 系统主题色变更事件
        /// </summary>
        event EventHandler AccentColorChanged;

        void StartMonitoringSystemUsesLightTheme();
        void StopMonitoringSystemUsesLightTheme();

        /// <summary>
        /// 监测系统主题色变更消息
        /// </summary>
        /// <param name="window"></param>
        void WatchWindowsColor(object window);

        void StartMonitoringWindowPrevalenceAccentColor();

        void StopMonitoringWindowPrevalenceAccentColor();

        /// <summary>
        /// 应用系统主题色到窗口
        /// </summary>
        void ApplyAccentColor();
    }
}
