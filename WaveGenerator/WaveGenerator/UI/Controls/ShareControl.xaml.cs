using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using WaveGenerator.UI.Interop;
using Windows.ApplicationModel;
using Windows.ApplicationModel.DataTransfer;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using WinRT;

namespace WaveGenerator.UI.Controls
{
    public sealed partial class ShareControl : UserControl
    {
        public ShareControl()
        {
            this.InitializeComponent();

            if (IsInDesignmode)
                return;

            //IntPtr hwnd = Process.GetCurrentProcess().MainWindowHandle;
            //DataTransferManager dataTransferManager = DataTransferManagerInterop.GetForWindow(hwnd);
            //dataTransferManager.DataRequested += DataTransferManager_DataRequested;
        }

        public Canvas MainCanvas { get; set; }
        public bool IsInDesignmode => DesignMode.DesignModeEnabled || DesignMode.DesignMode2Enabled;

        private async void DataTransferManager_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            DataRequest request = args.Request;
            DataRequestDeferral deferral = request.GetDeferral();

            RenderTargetBitmap renderTargetBitmap = await RenderImage();

            IRandomAccessStream stream = new InMemoryRandomAccessStream();
            await WriteImageToStream(renderTargetBitmap, stream);
            request.Data.SetBitmap(RandomAccessStreamReference.CreateFromStream(stream));

            var props = request.Data.Properties;
            props.ApplicationName = "Wave Generator";
            props.Title = "Welle teilen";
            props.Description = "Snapshot der aktuelle Welle teilen";

            deferral.Complete();
        }

        private void ShareButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            DataTransferManagerInterop.ShowShareUIForWindow(Process.GetCurrentProcess().MainWindowHandle);
        }

        private async void SaveButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            FileSavePicker picker = new FileSavePicker();

            picker.As<IInitializeWithWindow>().Initialize(Process.GetCurrentProcess().MainWindowHandle);
            // ((IInitializeWithWindow)(object)picker).Initialize(Process.GetCurrentProcess().MainWindowHandle);

            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            picker.FileTypeChoices.Add("Bild", new List<string>() { ".jpeg" });
            StorageFile file = await picker.PickSaveFileAsync();
            if (file != null)
            {
                RenderTargetBitmap renderTargetBitmap = await RenderImage();

                using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.ReadWrite))
                {
                    await WriteImageToStream(renderTargetBitmap, stream);
                }
            }
        }

        private async Task WriteImageToStream(RenderTargetBitmap bitmap, IRandomAccessStream stream)
        {
            byte[] pixels = (await bitmap.GetPixelsAsync()).ToArray();

            const int dpi = 150; // 96;

            BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, stream);
            encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied, (uint)bitmap.PixelWidth, (uint)bitmap.PixelHeight, dpi, dpi, pixels);
            await encoder.FlushAsync();

            stream.Seek(0);
        }

        private async Task<RenderTargetBitmap> RenderImage()
        {
            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap();
            await renderTargetBitmap.RenderAsync(MainCanvas);
            return renderTargetBitmap;
        }
    }
}
