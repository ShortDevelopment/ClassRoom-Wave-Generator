using WaveGenerator.UI.Generation;
using Microsoft.UI.Xaml.Controls;

namespace WaveGenerator.UI.Controls
{
    public sealed partial class WaveSettingsControl : UserControl
    {
        public WaveSettingsControl()
        {
            this.InitializeComponent();
        }

        public WaveSettings WaveSettings { get; set; }

        bool loadedSettings = false;

        public void LoadSettings()
        {
            WaveLengthTextBox.Text = WaveSettings.WaveLength.ToString();
            PeriodTextBox.Text = WaveSettings.Period.ToString();
            AmplitudeTextBox.Text = WaveSettings.Amplitude.ToString();

            OnlyOneWaveLength_CheckBox.IsChecked = WaveSettings.OnlyOneWaveLength;

            if (WaveSettings.Reflection != null)
            {
                ReflectionSettingsContainer.Visibility = Microsoft.UI.Xaml.Visibility.Visible;

                HasFreeEnd_CheckBox.IsChecked = WaveSettings.Reflection.HasFreeEnd;
                EndDistanceTextBox.Text = WaveSettings.Reflection.EndPosition.ToString();
            }
            else
            {
                ReflectionSettingsContainer.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
            }

            loadedSettings = true;
        }

        #region Settings TextBoxes        

        private void WaveLengthTextBox_KeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
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

        private void PeriodTextBox_KeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
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

        private void AmplitudeTextBox_KeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
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

        private void EndDistanceTextBox_KeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                try
                {
                    WaveSettings.Reflection.EndPosition = double.Parse(EndDistanceTextBox.Text);
                }
                catch { }
                LoadSettings();
            }
        }

        #endregion

        private void Settings_CheckBox_Checked(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            if (!loadedSettings)
                return;

            if (WaveSettings.Reflection != null)
                WaveSettings.Reflection.HasFreeEnd = (bool)HasFreeEnd_CheckBox.IsChecked;
            WaveSettings.OnlyOneWaveLength = (bool)OnlyOneWaveLength_CheckBox.IsChecked;
        }

    }
}
