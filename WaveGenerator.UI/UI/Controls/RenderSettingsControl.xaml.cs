using WaveGenerator.Rendering;
using Windows.UI.Xaml.Controls;

namespace WaveGenerator.UI.Controls
{
    public sealed partial class RenderSettingsControl : UserControl
    {
        public RenderSettingsControl()
        {
            this.InitializeComponent();
        }

        public RenderSettings RenderSettings { get; set; }

        bool loadedSettings = false;

        public void LoadSettings()
        {
            ShowIncomingWave_CheckBox.IsChecked = RenderSettings.ShowIncomingWave;
            ShowReflectedWave_CheckBox.IsChecked = RenderSettings.ShowReflectedWave;
            ShowResultingWave_CheckBox.IsChecked = RenderSettings.ShowResultingWave;

            loadedSettings = true;
        }

        private void ReflectionSettings_CheckBox_Checked(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (!loadedSettings)
                return;

            RenderSettings.ShowIncomingWave = (bool)ShowIncomingWave_CheckBox.IsChecked;
            RenderSettings.ShowReflectedWave = (bool)ShowReflectedWave_CheckBox.IsChecked;
            RenderSettings.ShowResultingWave = (bool)ShowResultingWave_CheckBox.IsChecked;
        }
    }
}
