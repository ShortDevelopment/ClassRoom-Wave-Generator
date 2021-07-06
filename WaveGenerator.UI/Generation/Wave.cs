using System.Numerics;
using Windows.UI;

namespace WaveGenerator.UI.Generation
{
    public struct Wave
    {
        public bool RTL { get; set; }
        public Vector2[] data { get; set; }

        public Color color { get; set; }

        public Wave(Vector2[] data, bool RTL = false)
        {
            this.data = data;
            this.RTL = RTL;
            this.color = Colors.Black;
        }
    }
}
