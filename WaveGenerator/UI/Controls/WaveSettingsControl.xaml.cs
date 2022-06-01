using WaveGenerator.Generation;
using Windows.UI.Xaml.Controls;

namespace WaveGenerator.UI.Controls
{
    public sealed partial class WaveSettingsControl : UserControl
    {
        public WaveSettingsControl()
        {
            this.InitializeComponent();

            WaveGenerationModeComboBox.ItemsSource = new UI.EnumItemSource<Generation.WaveGenerationMode>();
        }

        public WaveSettings WaveSettings { get; set; }

        bool loadedSettings = false;

        public void LoadSettings()
        {
            WaveLengthTextBox.Text = WaveSettings.WaveLength.ToString();
            PeriodTextBox.Text = WaveSettings.Period.ToString();
            AmplitudeTextBox.Text = WaveSettings.MaxElongation.ToString();

            WaveGenerationModeComboBox.SelectedIndex = 0;

            if (WaveSettings.Reflection != null)
            {
                ReflectionSettingsContainer.Visibility = Windows.UI.Xaml.Visibility.Visible;

                HasFreeEnd_CheckBox.IsChecked = WaveSettings.Reflection.HasFreeEnd;
                EndDistanceTextBox.Text = WaveSettings.Reflection.EndPosition.ToString();
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
                    WaveSettings.MaxElongation = double.Parse(AmplitudeTextBox.Text);
                }
                catch { }
                LoadSettings();
            }
        }

        private void EndDistanceTextBox_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
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

        private void Settings_CheckBox_Checked(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (!loadedSettings)
                return;

            if (WaveSettings.Reflection != null)
                WaveSettings.Reflection.HasFreeEnd = (bool)HasFreeEnd_CheckBox.IsChecked;            
        }

        private void WaveGenerationModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            WaveSettings.GenerationMode = (WaveGenerationModeComboBox.SelectedValue as EnumValueReference<WaveGenerationMode>).Value;
        }
    }
}
