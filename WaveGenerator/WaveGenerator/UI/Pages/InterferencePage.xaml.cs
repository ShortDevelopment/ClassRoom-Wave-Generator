using System.Diagnostics;
using System.Threading;
using WaveGenerator.UI.Generation;
using WaveGenerator.UI.Rendering;
using Microsoft.UI;

namespace WaveGenerator.UI.Pages
{
    public sealed partial class InterferencePage : SimulationPageBase
    {

        #region System // FrameWork

        public InterferencePage() : base()
        {
            this.InitializeComponent();
        }

        protected override void OnLoaded()
        {
            Wave1SettingsControl.LoadSettings();
            Wave2SettingsControl.LoadSettings();
            RenderSettingsControl.LoadSettings();
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

        public WaveSettings Wave2Settings { get; private set; } = new WaveSettings();

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
            var generater2 = new Generation.WaveGenerator(Wave2Settings);
            var renderer = new WaveRenderer(MainCanvas, RenderSettings);

            // Time animation intervals
            const int timeStep = 100;

            var CurrentDispatcher = DispatcherQueue;

            while (CurrentDispatcher != null)
            {
                if (!IsPageVisibleInFrame)
                    continue;

                // Sync settings
                RenderSettings.YStepCount = WaveSettings.Amplitude + 1;

                // Generate wave
                var primaryWave = generater.Generate(CurrentAnimationTime / 1000.0);
                var secondaryWave = generater2.Generate(CurrentAnimationTime / 1000.0);
                var resultingWave = generater.MergeWaves(new[] { primaryWave, secondaryWave });

                // Render wave
                _ = CurrentDispatcher.TryEnqueue(() =>
                {
                    double radius = WaveSettings.Amplitude * renderer.YUnit;

                    renderer.ClearCanvas();

                    renderer.RenderCoordinateSystem(Colors.Gray);

                    if (RenderSettings.ShowIncomingWave)
                        renderer.Render(primaryWave);
                    else
                        renderer.HideWave(primaryWave.color);

                    if (RenderSettings.ShowReflectedWave)
                        renderer.Render(secondaryWave);
                    else
                        renderer.HideWave(secondaryWave.color);

                    if (RenderSettings.ShowResultingWave)
                        renderer.Render(resultingWave);
                    else
                        renderer.HideWave(resultingWave.color);
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
