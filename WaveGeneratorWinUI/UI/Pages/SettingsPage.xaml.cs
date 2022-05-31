using System;
using System.Diagnostics;
using System.Reflection;
using Microsoft.UI.Xaml.Controls;

namespace WaveGenerator.UI.Pages
{
    public sealed partial class SettingsPage : Page
    {
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
