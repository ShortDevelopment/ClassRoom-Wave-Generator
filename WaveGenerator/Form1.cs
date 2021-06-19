using Microsoft.Toolkit.Forms.UI.XamlHost;
using System.Drawing;
using System.Windows.Forms;

namespace WaveGenerator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            SetStyle(ControlStyles.ResizeRedraw, true);
        }

        #region SplashScreen

        public bool IsSplashScreenVisible { get; private set; }
        public Color SplashScreenColor { get; } = Color.LightGray;

        public void ShowSplashScreen()
        {
            IsSplashScreenVisible = true;
            this.Refresh();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (IsSplashScreenVisible)
            {
                using (Graphics g = CreateGraphics())
                {
                    g.Clear(SplashScreenColor);
                    const int IconWidth = 100;
                    g.DrawIcon(this.Icon, new Rectangle(this.Width / 2 - IconWidth / 2, this.Height / 2 - IconWidth / 2, IconWidth, IconWidth));
                }
            }
            else
            {
                base.OnPaint(e);
            }
        }

        #endregion

        #region XamlContent

        public void LoadXamlContent()
        {
            IsSplashScreenVisible = false;
            this.Refresh();

            if (!DesignMode)
            {
                var XamlIsland = new WindowsXamlHost();
                XamlIsland.Dock = DockStyle.Fill;
                XamlIsland.Child = new UI.MainPage();
                this.Controls.Add(XamlIsland);
            }
        }

        #endregion

    }
}
