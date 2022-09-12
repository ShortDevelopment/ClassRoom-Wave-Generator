using System;
using System.Diagnostics;
using System.Reflection;
using Windows.UI.Xaml.Controls;

namespace WaveGenerator.UI.Pages
{
    public sealed partial class SettingsPage : Page
    {
        public static bool LabelWavePoints { get; set; } = false;
        public static bool InterpolateWavePoints { get; set; } = false;

        /// <summary>
        /// Sets radius of each circle in wave display
        /// </summary>
        public static double WavePointRadius { get; set; } = 4;

        public static double WavePointDistance { get; set; } = 0.25;

        public SettingsPage()
        {
            this.InitializeComponent();
        }

        public string UI_CurrentYear { get; } = DateTime.Now.ToString("yyyy");

        public FileVersionInfo UI_VersionInfo
        {
            get
            {
                try
                {
                    return FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
    }
}
