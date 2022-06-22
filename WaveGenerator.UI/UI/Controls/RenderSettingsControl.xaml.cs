using WaveGenerator.Rendering;
using Windows.UI.Xaml;
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

        public bool ShowReflectionSettings { get; set; } = true;

        bool loadedSettings = false;
        public void LoadSettings()
        {
            if (ShowReflectionSettings)
                ReflectionRenderSettingsPanel.Visibility = Visibility.Visible;
            else
                ReflectionRenderSettingsPanel.Visibility = Visibility.Collapsed;

            ShowIncomingWave_CheckBox.IsChecked = RenderSettings.ShowIncomingWave;
            ShowReflectedWave_CheckBox.IsChecked = RenderSettings.ShowReflectedWave;
            ShowResultingWave_CheckBox.IsChecked = RenderSettings.ShowResultingWave;

            YStepCountTextBox.Text = RenderSettings.YStepCount.ToString();

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

        private void YStepCountTextBox_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (!loadedSettings)
                return;

            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                try
                {
                    RenderSettings.YStepCount = int.Parse(YStepCountTextBox.Text);
                }
                catch { }
                LoadSettings();
            }
        }
    }
}
