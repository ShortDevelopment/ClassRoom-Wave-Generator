using Microsoft.Toolkit.Forms.UI.XamlHost;
using System.Windows.Forms;

namespace WaveGenerator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            if (!DesignMode)
            {
                var XamlIsland = new WindowsXamlHost();
                XamlIsland.Dock = DockStyle.Fill;
                XamlIsland.Child = new UI.MainPage();
                this.Controls.Add(XamlIsland);
            }
        }

    }
}
