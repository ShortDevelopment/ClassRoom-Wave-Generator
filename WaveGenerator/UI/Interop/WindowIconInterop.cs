using Microsoft.UI.Xaml;
using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using WinRT;

namespace WaveGenerator.UI.Interop
{
    public static class WindowIconInterop
    {
        public static void SetIcon(this Window window, string resourceName)
        {
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            {
                IntPtr hWnd = window.As<IWindowNative>().WindowHandle;
                Icon icon = new(stream);
                SendMessage(hWnd, WM_SETICON, IntPtr.Zero, icon.Handle);
            }
        }


        private const uint WM_SETICON = 0x0080;

        [DllImport("user32")]
        private static extern void SendMessage(IntPtr hWnd, uint msgId, IntPtr reserved, IntPtr hIcon);

        /// <summary>
        /// <see href="https://github.com/microsoft/microsoft-ui-xaml/issues/4056"/>
        /// </summary>
        [ComImport]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [Guid("EECDBF0E-BAE9-4CB6-A68E-9598E1CB57BB")]
        internal interface IWindowNative
        {
            IntPtr WindowHandle { get; }
        }
    }
}
