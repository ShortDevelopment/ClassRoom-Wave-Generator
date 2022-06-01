using Microsoft.UI;
using System.Linq;
using System.Numerics;
using Windows.UI;

namespace WaveGenerator.Generation
{
    public struct Wave
    {
        public Vector2[] data { get; set; }
        public Color color { get; set; }
        public double time { get; set; }

        public Wave(Vector2[] data, double time)
        {
            this.data = data;
            this.time = time;
            this.color = Colors.Black;
        }

        public void Reverse(float endDistance)
        {
            data = data.Select((x) => new Vector2(endDistance - x.X, x.Y)).ToArray();
        }
    }
}
