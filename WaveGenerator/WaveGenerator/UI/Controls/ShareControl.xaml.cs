﻿using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using WaveGenerator.UI.Interop;
using Windows.ApplicationModel;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
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

            this.Loaded += ShareControl_Loaded;

            //IntPtr hwnd = Process.GetCurrentProcess().MainWindowHandle;
            //DataTransferManager dataTransferManager = DataTransferManagerInterop.GetForWindow(hwnd);
            //dataTransferManager.DataRequested += DataTransferManager_DataRequested;
        }

        private void ShareControl_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            MainCanvas.Clip = new RectangleGeometry { Rect = new Rect(0, 0, MainCanvas.ActualWidth, MainCanvas.ActualHeight) };
        }

        public Canvas MainCanvas { get; set; }
        public bool IsInDesignmode => DesignMode.DesignModeEnabled || DesignMode.DesignMode2Enabled;

        private async void DataTransferManager_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            DataRequest request = args.Request;
            DataRequestDeferral deferral = request.GetDeferral();

            IRandomAccessStream stream = new InMemoryRandomAccessStream();
            await RenderCanvasToStream(MainCanvas, stream);
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
            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            picker.FileTypeChoices.Add("Bild", new List<string>() { ".jpeg" });

            StorageFile file = await picker.PickSaveFileAsync();
            if (file != null)
                using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.ReadWrite))
                    await RenderCanvasToStream(MainCanvas, stream);
        }

        private async void CopyButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            IRandomAccessStream stream = new InMemoryRandomAccessStream();
            await RenderCanvasToStream(MainCanvas, stream);

            DataPackage data = new();
            data.SetBitmap(RandomAccessStreamReference.CreateFromStream(stream));
            Clipboard.SetContent(data);
        }

        private async Task RenderCanvasToStream(Canvas canvas, IRandomAccessStream stream)
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
