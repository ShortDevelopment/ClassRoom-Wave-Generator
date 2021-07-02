using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace WaveGenerator
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.RealTime;

            Form1 MainForm = new Form1();
            MainForm.Show();
            MainForm.ShowSplashScreen();

            Application.DoEvents();

            using (new UI.App())
            {
                MainForm.LoadXamlContent();
                Application.Run(MainForm);
            }
        }
    }
}
