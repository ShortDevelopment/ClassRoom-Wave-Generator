using Microsoft.Graphics.Canvas;
using Microsoft.Toolkit.Uwp.UI.Media.Geometry;
using System;
using System.Numerics;
using Windows.Devices.Geolocation;
using Windows.UI;

namespace WaveGenerator.Rendering
{
    internal static class Extensions
    {
        public static void DrawArrow(this CanvasDrawingSession @this, Vector2 start, Vector2 end, Color color, float strokeThickness = 1, float tipLength = 5, float angleDeg = 45)
        {
            // base
            @this.DrawLine(start, end, color, strokeThickness);

            Vector2 dir = -(end - start).Normalized();

            // tip
            @this.DrawLine(end, end + dir.Rotate(angleDeg) * tipLength, color, strokeThickness);
            @this.DrawLine(end, end + dir.Rotate(-angleDeg) * tipLength, color, strokeThickness);
        }

        public static Vector2 Normalized(this Vector2 @this)
            => @this / @this.Length();

        public static Vector2 Rotate(this Vector2 @this, float angleDeg)
        {
            float angleRad = angleDeg / 180f * MathF.PI;
            return new(
                @this.X * MathF.Cos(angleRad) - @this.Y * MathF.Sin(angleRad),
                @this.X * MathF.Sin(angleRad) + @this.Y * MathF.Cos(angleRad)
            );
        }
    }
}
