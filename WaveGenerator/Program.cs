using System;
using System.Windows.Forms;

namespace WaveGenerator
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            using (new UI.App())
            {
                Application.SetHighDpiMode(HighDpiMode.SystemAware);
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1());
            }
        }
    }
}
