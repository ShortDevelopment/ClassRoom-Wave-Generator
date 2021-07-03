using Microsoft.Graphics.Canvas;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using WaveGenerator.UI.Generation;
using WaveGenerator.UI.Rendering;
using Windows.ApplicationModel.DataTransfer;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace WaveGenerator.UI
{
    public sealed partial class DefaultWavePage : Page
    {

        #region System // FrameWork

        public DefaultWavePage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Required;

            IntPtr hwnd = Process.GetCurrentProcess().MainWindowHandle;
            DataTransferManager dataTransferManager = Interop.DataTransferManagerInterop.GetForWindow(hwnd);
            dataTransferManager.DataRequested += DataTransferManager_DataRequested;

            this.Loaded += MainPage_Loaded;
        }

        private void MainPage_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            WaveSettingsControl.LoadSettings();

            this.KeyDown += MainPage_KeyDown;

            Task.Run(RenderLoop);
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
                // Sync settings
                generater.Settings = WaveSettings;
                RenderSettings.YStepCount = WaveSettings.Amplitude + 1;
                renderer.Settings = RenderSettings;

                // Generate wave
                var data = generater.Generate(CurrentAnimationTime / 1000.0);

                // Generate "Zeiger"
                var angle = generater.CalculateZeigerAngle(CurrentAnimationTime / 1000.0);

                // Render wave
                CurrentDispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, () =>
                {
                    double radius = WaveSettings.Amplitude * renderer.YUnit;
                    RenderSettings.Offset = new System.Numerics.Vector2((float)radius * 2, 0);

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

        #region UI        

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
            InvokeSingleStep();
        }

        private void ResetAppBarButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            CurrentAnimationTime = 0;

            if (!IsRunning)
                ResetAppBarButton.IsEnabled = false;
        }

        #endregion

        private void InvokeSingleStep()
        {
            CurrentAnimationTime += (int)Math.Round((1f / 16f) * WaveSettings.Period * 1000);
        }
        private void InvokeSingleStepReverse()
        {
            CurrentAnimationTime -= (int)Math.Round((1f / 16f) * WaveSettings.Period * 1000);
        }

        #endregion

        #region Share & Export

        private void ShareButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            // DataTransferManager.ShowShareUI();
            Interop.DataTransferManagerInterop.ShowShareUIForWindow(Process.GetCurrentProcess().MainWindowHandle);
        }

        private async void DataTransferManager_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            DataRequest request = args.Request;

            DataRequestDeferral deferral = request.GetDeferral();

            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap();
            await renderTargetBitmap.RenderAsync(MainCanvas);

            var pixels = (await renderTargetBitmap.GetPixelsAsync()).ToArray();

            IRandomAccessStream stream = new InMemoryRandomAccessStream();

            var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);
            encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied, (uint)renderTargetBitmap.PixelWidth, (uint)renderTargetBitmap.PixelHeight, 96, 96, pixels);
            await encoder.FlushAsync();
            stream.Seek(0);
            request.Data.SetBitmap(RandomAccessStreamReference.CreateFromStream(stream));

            var props = request.Data.Properties;
            props.ApplicationName = "Wave Generator";
            props.Title = "Welle teilen";
            props.Description = "Snapshot der aktuelle Welle teilen";

            deferral.Complete();
        }

        #endregion

    }
}
