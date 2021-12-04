using Microsoft.UI;
using System.Numerics;
using Windows.UI;

namespace WaveGenerator.Generation
{
    public struct Wave
    {
        public Vector2[] data { get; set; }
        public Color color { get; set; }

        public Wave(Vector2[] data)
        {
            this.data = data;
            this.color = Colors.Black;
        }
    }
}
