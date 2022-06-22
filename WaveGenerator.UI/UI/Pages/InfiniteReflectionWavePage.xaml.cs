using Windows.UI;
using System.Threading;
using WaveGenerator.Generation;
using WaveGenerator.Rendering;
using Colors = Windows.UI.Colors;

namespace WaveGenerator.UI.Pages
{
    public sealed partial class InfiniteReflectionWavePage : SimulationPageBase
    {

        #region System // FrameWork

        public InfiniteReflectionWavePage() : base()
        {
            this.InitializeComponent();
            WaveSettings.Reflection = new WaveReflectionInfo();
        }

        protected override void OnLoaded()
        {
            WaveSettingsControl.LoadSettings();
            RenderSettingsControl.LoadSettings();
            this.KeyDown += MainPage_KeyDown;
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

            // Time animation intervals
            const int timeStep = 100;

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
                var data = generater.GenerateReflectedWaveBothSides(CurrentAnimationTime / 1000.0);

                // Render wave
                _ = CurrentDispatcher.RunIdleAsync((x) =>
                  {
                      renderer.ClearCanvas();

                      renderer.RenderCoordinateSystem(Colors.Gray);

                      renderer.Render(data);
                      renderer.RenderReflectionWall(WaveSettings.Reflection);
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
