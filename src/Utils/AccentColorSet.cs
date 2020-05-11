/// https://github.com/Code-Inside/Samples/blob/master/2016/WpfGetWindows10AccentColor/WpfApplication1/MainWindow.xaml.cs

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Media;

namespace SimpleDICOMToolkit.Utils
{
    // TODO: Add a listener for WM_SETTINGCHANGE to detect changes of the active color scheme automatically.
    //   Add a listener for WM_SETTINGCHANGE and trigger an event, like ActiveSetChanged.
    public class AccentColorSet
    {
        public static AccentColorSet[] AllSets
        {
            get
            {
                if (_allSets == null)
                {
                    uint colorSetCount = UXTheme.GetImmersiveColorSetCount();

                    List<AccentColorSet> colorSets = new List<AccentColorSet>();
                    for (uint i = 0; i < colorSetCount; i++)
                    {
                        colorSets.Add(new AccentColorSet(i, false));
                    }

                    AllSets = colorSets.ToArray();
                }

                return _allSets;
            }
            private set
            {
                _allSets = value;
            }
        }

        public static AccentColorSet ActiveSet
        {
            get
            {
                uint activeSet = UXTheme.GetImmersiveUserColorSetPreference(false, false);
                ActiveSet = AllSets[Math.Min(activeSet, AllSets.Length - 1)];
                return _activeSet;
            }
            private set
            {
                if (_activeSet != null) _activeSet.Active = false;

                value.Active = true;
                _activeSet = value;
            }
        }

        public bool Active { get; private set; }

        public Color this[string colorName]
        {
            get
            {
                IntPtr name = IntPtr.Zero;
                uint colorType;

                try
                {
                    name = Marshal.StringToHGlobalUni("Immersive" + colorName);
                    colorType = UXTheme.GetImmersiveColorTypeFromName(name);
                    if (colorType == 0xFFFFFFFF) throw new InvalidOperationException();
                }
                finally
                {
                    if (name != IntPtr.Zero)
                    {
                        Marshal.FreeHGlobal(name);
                    }
                }

                return this[colorType];
            }
        }

        public Color this[uint colorType]
        {
            get
            {
                uint nativeColor = UXTheme.GetImmersiveColorFromColorSetEx(this._colorSet, colorType, false, 0);
                //if (nativeColor == 0)
                //    throw new InvalidOperationException();
                return Color.FromArgb(
                    (byte)((0xFF000000 & nativeColor) >> 24),
                    (byte)((0x000000FF & nativeColor) >> 0),
                    (byte)((0x0000FF00 & nativeColor) >> 8),
                    (byte)((0x00FF0000 & nativeColor) >> 16)
                    );
            }
        }

        AccentColorSet(uint colorSet, bool active)
        {
            this._colorSet = colorSet;
            this.Active = active;
        }

        static AccentColorSet[] _allSets;
        static AccentColorSet _activeSet;
        readonly uint _colorSet;

        // HACK: GetAllColorNames collects the available color names by brute forcing the OS function.
        //   Since there is currently no known way to retrieve all possible color names,
        //   the method below just tries all indices from 0 to 0xFFF ignoring errors.
        public List<string> GetAllColorNames()
        {
            List<string> allColorNames = new List<string>();
            for (uint i = 0; i < 0xFFF; i++)
            {
                IntPtr typeNamePtr = UXTheme.GetImmersiveColorNamedTypeByIndex(i);
                if (typeNamePtr != IntPtr.Zero)
                {
                    IntPtr typeName = (IntPtr)Marshal.PtrToStructure(typeNamePtr, typeof(IntPtr));
                    allColorNames.Add(Marshal.PtrToStringUni(typeName));
                }
            }

            return allColorNames;
        }

        static class UXTheme
        {
            [DllImport("uxtheme.dll", EntryPoint = "#98", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
            public static extern uint GetImmersiveUserColorSetPreference(bool forceCheckRegistry, bool skipCheckOnFail);

            [DllImport("uxtheme.dll", EntryPoint = "#94", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
            public static extern uint GetImmersiveColorSetCount();

            [DllImport("uxtheme.dll", EntryPoint = "#95", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
            public static extern uint GetImmersiveColorFromColorSetEx(uint immersiveColorSet, uint immersiveColorType,
                bool ignoreHighContrast, uint highContrastCacheMode);

            [DllImport("uxtheme.dll", EntryPoint = "#96", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
            public static extern uint GetImmersiveColorTypeFromName(IntPtr name);

            [DllImport("uxtheme.dll", EntryPoint = "#100", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
            public static extern IntPtr GetImmersiveColorNamedTypeByIndex(uint index);
        }
    }
}
