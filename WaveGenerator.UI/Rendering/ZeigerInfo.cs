using System.Numerics;
using Windows.UI;

namespace WaveGenerator.Rendering
{
    public sealed record ZeigerInfo(double Angle, Vector2 Position, float Radius)
    {
        public Color Color { get; set; } = Colors.Green;
        public bool ShowAmplitude { get; set; } = true;
    }
}

namespace System.Runtime.CompilerServices
{
    internal sealed class IsExternalInit { }
}
