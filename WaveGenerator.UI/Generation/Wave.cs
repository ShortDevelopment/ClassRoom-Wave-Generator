using System.Numerics;

namespace WaveGenerator.UI.Generation
{
    public struct Wave
    {
        public bool isRTL;
        public Vector2[] data;

        public Wave(Vector2[] data, bool isRTL = false)
        {
            this.data = data;
            this.isRTL = isRTL;
        }
    }
}
