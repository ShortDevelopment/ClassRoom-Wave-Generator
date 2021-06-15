using System;
using System.Threading;
using System.Threading.Tasks;
using WaveGenerator.UI.Generation;
using WaveGenerator.UI.Rendering;
using Windows.UI.Xaml.Controls;

namespace WaveGenerator.UI
{
    public sealed partial class MainPage : Page
    {

        #region System // FrameWork

        public MainPage()
        {
            this.InitializeComponent();

            this.Loaded += MainPage_Loaded;
        }

        private void MainPage_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            LoadSettings();
            Task.Run(RenderLoop);
        }

        #endregion

        #region Animation

        #region State Variables

        /// <summary>
        /// Gets wether animation is running
        /// </summary>
        public bool IsRunning { get; private set; } = false;

        /// <summary>
        /// Time in ms
        /// </summary>
        public int CurrentAnimationTime { get; private set; } = 0;

        /// <summary>
        /// Internally used to stop <see cref="MainPage.RenderLoop"/>
        /// </summary>
        bool ShouldStopAnimation = false;

        #endregion

        public WaveSettings WaveSettings { get; private set; } = new WaveSettings();
        public RenderSettings RenderSettings { get; private set; } = new RenderSettings();

        /// <summary>
        /// Handles rendering calls and time
        /// </summary>
        private void RenderLoop()
        {
            var generater = new Generation.WaveGenerator(WaveSettings);
            var renderer = new WaveRenderer(MainCanvas, RenderSettings);

            const int timeStep = 100;

            var CurrentDispatcher = Dispatcher;

            while (CurrentDispatcher != null)
            {
                // Sync settings
                generater.Settings = WaveSettings;
                RenderSettings.YStepCount = WaveSettings.Amplitude + 1;
                renderer.Settings = RenderSettings;

                // Generate wave
                var data = generater.Generate(CurrentAnimationTime / 1000.0);

                // Render wave
                CurrentDispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, () => { renderer.Render(data); });

                // Handling timing
                if (IsRunning)
                {
                    CurrentAnimationTime += timeStep;
                }
                Thread.Sleep(timeStep);
            }
        }

        #endregion

        #region UI

        public string UI_CurrentYear { get; } = DateTime.Now.ToString("yyyy");

        void LoadSettings()
        {
            WaveLengthTextBox.Text = WaveSettings.WaveLength.ToString();
            PeriodTextBox.Text = WaveSettings.Period.ToString();
            AmplitudeTextBox.Text = WaveSettings.Amplitude.ToString();
        }

        #region CommandBar

        private void PlayAppBarButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            IsRunning = true;

            PlayAppBarButton.IsEnabled = false;
            PauseAppBarButton.IsEnabled = true;

            ResetAppBarButton.IsEnabled = true;
        }

        private void PauseAppBarButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            IsRunning = false;

            PlayAppBarButton.IsEnabled = true;
            PauseAppBarButton.IsEnabled = false;
        }

        private void StepAppBarButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            CurrentAnimationTime += 1000;
        }

        #endregion

        #region Settings TextBoxes

        private void ResetAppBarButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            CurrentAnimationTime = 0;

            if (!IsRunning)
                ResetAppBarButton.IsEnabled = false;
        }

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

        #endregion
    }
}
