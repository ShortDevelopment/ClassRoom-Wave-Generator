using ShortDev.Uwp.FullTrust.Xaml;
using System.Runtime.InteropServices;
using System;
using WinUI.Interop.CoreWindow;
using WinUI.Interop.NativeWindow;

namespace WaveGenerator.Launcher
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (XamlApplicationWrapper appWrapper = new(() => new App()))
            {
                var window = XamlWindowActivator.CreateNewWindow(new("WaveGenerator")
                {
                    HasWin32Frame = true
                });
                window.Content = new UI.MainPage();

                var hwnd = window.CoreWindow.GetHwnd();
                WindowExtensions.SetIcon(hwnd, "WaveGenerator.Logo.ico");
                PostMessage(hwnd, 0x270, 0, 1);

                XamlWindowSubclass.ForWindow(window).CurrentFrameworkView!.Run();
            }
        }

        [DllImport("User32.Dll")]
        public static extern bool PostMessage(IntPtr hWnd, uint msg, int wParam, int lParam);
    }
}
