using System.Diagnostics;
using System.Threading;
using WaveGenerator.UI.Generation;
using WaveGenerator.UI.Rendering;
using Windows.UI;

namespace WaveGenerator.UI.Pages
{
    public sealed partial class ReflectionWavePage : SimulationPageBase
    {

        #region System // FrameWork

        public ReflectionWavePage() : base()
        {
            this.InitializeComponent();

            WaveSettings.Reflection = new WaveReflectionInfo();
        }

        protected override void OnLoaded()
        {
            WaveSettingsControl.LoadSettings();
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

        Stopwatch watch = new Stopwatch();

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
                var primaryWave = generater.Generate(CurrentAnimationTime / 1000.0);
                var secondaryWave = generater.MirrorWave(primaryWave);
                var resultingWave = generater.MergeWaves(new[] { primaryWave, secondaryWave });

                // Render wave
                _ = CurrentDispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    double radius = WaveSettings.Amplitude * renderer.YUnit;

                    renderer.ClearCanvas();

                    renderer.RenderCoordinateSystem(Colors.Gray);

                    if (RenderSettings.ShowIncomingWave)
                        renderer.Render(primaryWave);
                    if (RenderSettings.ShowReflectedWave)
                        renderer.Render(secondaryWave);
                    if (RenderSettings.ShowResultingWave)
                        renderer.Render(resultingWave);

                    renderer.RenderReflectionWall(WaveSettings.Reflection);
                });

                // Handling timing
                if (IsRunning)
                {
                    CurrentAnimationTime += (int)watch.ElapsedMilliseconds;
                }

                watch.Reset();
                watch.Start();
                Thread.Sleep(timeStep);
                watch.Stop();
            }
        }

        #endregion

    }
}
