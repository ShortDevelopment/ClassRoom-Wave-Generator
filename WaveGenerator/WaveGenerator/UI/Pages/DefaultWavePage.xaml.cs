using Microsoft.UI;
using System.Threading;
using WaveGenerator.UI.Generation;
using WaveGenerator.UI.Rendering;

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
            this.KeyDown += MainPage_KeyDown;
        }

        private void MainPage_KeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
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

            // Time animation intervals
            const int timeStep = 100;

            var CurrentDispatcher = Dispatcher;

            while (CurrentDispatcher != null)
            {
                if (!IsPageVisibleInFrame)
                    continue;

                // Sync settings
                generater.Settings = WaveSettings;
                RenderSettings.YStepCount = WaveSettings.Amplitude + 1;
                renderer.Settings = RenderSettings;

                // Generate wave
                var data = generater.Generate(CurrentAnimationTime / 1000.0);

                // Generate "Zeiger"
                var angle = generater.CalculateZeigerAngle(CurrentAnimationTime / 1000.0);

                // Render wave
                _ = CurrentDispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, () =>
                  {
                      double radius = WaveSettings.Amplitude * renderer.YUnit;
                      RenderSettings.Offset = new System.Numerics.Vector2((float)radius * 2, 0);

                      renderer.ClearCanvas();

                      renderer.RenderCoordinateSystem(Colors.Gray);

                      renderer.Render(data);
                      renderer.RenderZeiger(angle, new System.Numerics.Vector2(0, 0), radius);
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
