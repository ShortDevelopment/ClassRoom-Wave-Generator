using WaveGenerator.Generation;
using Windows.UI;

namespace WaveGenerator.Rendering
{
    public sealed record WallInfo(double EndPosition, Color Color)
    {
        public static WallInfo FromReflectionInfo(WaveReflectionInfo reflectionInfo)
            => new(reflectionInfo.EndPosition, Colors.Red);
    }
}
