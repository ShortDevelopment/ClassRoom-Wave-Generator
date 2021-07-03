using System;
using System.Diagnostics;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.DataTransfer;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace WaveGenerator.UI.Controls
{
    public sealed partial class ShareControl : UserControl
    {
        public ShareControl()
        {
            this.InitializeComponent();

            if (IsInDesignmode)
                return;

            IntPtr hwnd = Process.GetCurrentProcess().MainWindowHandle;
            DataTransferManager dataTransferManager = Interop.DataTransferManagerInterop.GetForWindow(hwnd);
            dataTransferManager.DataRequested += DataTransferManager_DataRequested;
        }

        public Canvas MainCanvas { get; set; }
        public bool IsInDesignmode => DesignMode.DesignModeEnabled || DesignMode.DesignMode2Enabled;

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

        private void ShareButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            // DataTransferManager.ShowShareUI();
            Interop.DataTransferManagerInterop.ShowShareUIForWindow(Process.GetCurrentProcess().MainWindowHandle);
        }

    }
}
