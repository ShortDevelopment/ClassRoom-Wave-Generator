using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using WinUI.Interop.CoreWindow;
using Microsoft.Graphics.Canvas.UI.Xaml;

namespace WaveGenerator.UI.Controls
{
    public sealed partial class ShareControl : UserControl
    {
        public ShareControl()
        {
            this.InitializeComponent();

            if (IsInDesignmode)
                return;

            DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += DataTransferManager_DataRequested;
        }

        public CanvasControl MainCanvas { get; set; }
        public bool IsInDesignmode => DesignMode.DesignModeEnabled || DesignMode.DesignMode2Enabled;

        private async void DataTransferManager_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            DataRequest request = args.Request;
            DataRequestDeferral deferral = request.GetDeferral();

            TaskCompletionSource<bool> promise = new();
            _ = Dispatcher.RunIdleAsync(async (x) =>
            {
                IRandomAccessStream stream = new InMemoryRandomAccessStream();
                await RenderCanvasToStream(stream);
                request.Data.SetBitmap(RandomAccessStreamReference.CreateFromStream(stream));

                var props = request.Data.Properties;
                props.ApplicationName = "Wave Generator";
                props.Title = "Welle teilen";
                props.Description = "Snapshot der aktuelle Welle teilen";

                promise.SetResult(true);
            });
            await promise.Task;

            deferral.Complete();
        }

        private void ShareButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            DataTransferManager.ShowShareUI();
        }

        private async void SaveButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            FileSavePicker picker = new();
            (picker as object as IInitializeWithWindow).Initialize(Process.GetCurrentProcess().MainWindowHandle);
            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            picker.FileTypeChoices.Add("Bild", new List<string>() { ".jpeg" });

            StorageFile file = await picker.PickSaveFileAsync();
            if (file != null)
                using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.ReadWrite))
                    await RenderCanvasToStream(stream);
        }

        private async void CopyButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            IRandomAccessStream stream = new InMemoryRandomAccessStream();
            await RenderCanvasToStream(stream);

            DataPackage data = new();
            data.SetBitmap(RandomAccessStreamReference.CreateFromStream(stream));
            Clipboard.SetContent(data);
        }

        private async Task RenderCanvasToStream(IRandomAccessStream stream)
        {
            RenderTargetBitmap bitmap = new RenderTargetBitmap();
            await bitmap.RenderAsync(MainCanvas);

            byte[] pixels = (await bitmap.GetPixelsAsync()).ToArray();

            const int dpi = 150; // 96;

            BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, stream);
            encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied, (uint)bitmap.PixelWidth, (uint)bitmap.PixelHeight, dpi, dpi, pixels);
            // encoder.BitmapTransform.Bounds = new(0, 0, (uint)MainCanvas.ActualWidth * dpi, (uint)MainCanvas.ActualHeight * dpi);
            await encoder.FlushAsync();

            stream.Seek(0);
        }
    }
}
