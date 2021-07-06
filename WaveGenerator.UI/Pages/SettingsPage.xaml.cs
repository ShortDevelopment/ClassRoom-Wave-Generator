using System;
using Windows.UI.Xaml.Controls;

namespace WaveGenerator.UI.Pages
{
    public sealed partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            this.InitializeComponent();
        }

        public string UI_CurrentYear { get; } = DateTime.Now.ToString("yyyy");
    }
}
