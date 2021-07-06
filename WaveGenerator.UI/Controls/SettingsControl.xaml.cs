using WaveGenerator.UI.Generation;
using WaveGenerator.UI.Rendering;
using Windows.UI.Xaml.Controls;

namespace WaveGenerator.UI.Controls
{
    public sealed partial class WaveSettingsControl : UserControl
    {
        public WaveSettingsControl()
        {
            this.InitializeComponent();
        }

        public WaveSettings WaveSettings { get; set; }
        public RenderSettings RenderSettings { get; set; }

        bool loadedSettings = false;

        public void LoadSettings()
        {
            WaveLengthTextBox.Text = WaveSettings.WaveLength.ToString();
            PeriodTextBox.Text = WaveSettings.Period.ToString();
            AmplitudeTextBox.Text = WaveSettings.Amplitude.ToString();

            if (WaveSettings.Reflection != null)
            {
                ReflectionSettingsContainer.Visibility = Windows.UI.Xaml.Visibility.Visible;

                HasFreeEnd_CheckBox.IsChecked = WaveSettings.Reflection.HasFreeEnd;

                ShowIncomingWave_CheckBox.IsChecked = RenderSettings.ShowIncomingWave;
                ShowReflectedWave_CheckBox.IsChecked = RenderSettings.ShowReflectedWave;
                ShowResultingWave_CheckBox.IsChecked = RenderSettings.ShowResultingWave;
            }
            else
            {
                ReflectionSettingsContainer.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }

            loadedSettings = true;
        }

        #region Settings TextBoxes        

        private void WaveLengthTextBox_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                try
                {
                    WaveSettings.WaveLength = double.Parse(WaveLengthTextBox.Text);
                }
                catch { }
                LoadSettings();
            }
        }

        private void PeriodTextBox_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                try
                {
                    WaveSettings.Period = double.Parse(PeriodTextBox.Text);
                }
                catch { }
                LoadSettings();
            }
        }

        private void AmplitudeTextBox_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                try
                {
                    WaveSettings.Amplitude = double.Parse(AmplitudeTextBox.Text);
                }
                catch { }
                LoadSettings();
            }
        }

        #endregion

        private void ReflectionSettings_CheckBox_Checked(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (!loadedSettings)
                return;

            WaveSettings.Reflection.HasFreeEnd = (bool)HasFreeEnd_CheckBox.IsChecked;

            RenderSettings.ShowIncomingWave = (bool)ShowIncomingWave_CheckBox.IsChecked;
            RenderSettings.ShowReflectedWave = (bool)ShowReflectedWave_CheckBox.IsChecked;
            RenderSettings.ShowResultingWave = (bool)ShowResultingWave_CheckBox.IsChecked;
        }
    }
}
