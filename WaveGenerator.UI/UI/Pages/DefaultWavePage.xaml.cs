using System.Threading;
using WaveGenerator.Generation;
using WaveGenerator.Rendering;
using Windows.UI.Xaml;

namespace WaveGenerator.UI.Pages
{
    public sealed partial class DefaultWavePage : SimulationPageBase
    {
        #region System // FrameWork

        public DefaultWavePage() : base()
        {
            this.InitializeComponent();
        }

        protected override void OnLoaded()
        {
            WaveSettingsControl.LoadSettings();
            RenderSettingsControl.LoadSettings();
            this.KeyDown += MainPage_KeyDown;
            var test = Window.Current.Visible;
        }

        private void MainPage_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Left)
            {
                InvokeSingleStepReverse();
            }
            else if (e.Key == Windows.System.VirtualKey.Right)
            {
                InvokeSingleStep();
            }
        }

        #endregion

        #region Animation

        /// <summary>
        /// Handles rendering calls and time
        /// </summary>
        protected override void RenderLoop()
        {
            // Setting high priority
            Thread.CurrentThread.Priority = ThreadPriority.Highest;

            // Initializing helpers
            var generater = new Generation.WaveGenerator(WaveSettings);
            var renderer = new WaveRenderer(MainCanvas, RenderSettings);

            var CurrentDispatcher = Dispatcher;

            while (CurrentDispatcher != null)
            {
                if (!IsPageVisibleInFrame)
                    continue;

                // Sync settings
                generater.Settings = WaveSettings;
                // RenderSettings.YStepCount = WaveSettings.Amplitude + 1;
                renderer.Settings = RenderSettings;

                // Generate wave
                var data = generater.Generate(CurrentAnimationTime / 1000.0);

                // Generate "Zeiger"
                var angle = generater.CalculateZeigerAngle(0, CurrentAnimationTime / 1000.0);

                // Render wave
                _ = CurrentDispatcher.RunIdleAsync((x) =>
                  {
                      float radius = (float)(WaveSettings.Amplitude * renderer.YUnit);
                      RenderSettings.Offset = new System.Numerics.Vector2(radius * 2, 0);

                      renderer.Clear();

                      renderer.ShowCoordinateSystem = true;

                      renderer.VisibleWaves.Add(data);
                      renderer.VisibleZeiger.Add(new(angle, new(0, 0), radius)
                      {
                          ShowAmplitude = ShowAmplitude_CheckBox.IsChecked == true
                      });

                      renderer.Render();
                  });

                // Handling timing
                if (IsRunning)
                {
                    CurrentAnimationTime += timeStep;
                }
                Thread.Sleep(timeStep);
            }
        }

        #endregion
    }
}
