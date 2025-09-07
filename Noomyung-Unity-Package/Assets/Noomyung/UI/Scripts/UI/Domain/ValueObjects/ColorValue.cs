using System;

namespace Noomyung.UI.Domain.ValueObjects
{
    /// <summary>
    /// Unity에 종속되지 않는 색상 값 객체입니다.
    /// </summary>
    public readonly struct ColorValue : IEquatable<ColorValue>
    {
        public float R { get; }
        public float G { get; }
        public float B { get; }
        public float A { get; }

        public ColorValue(float r, float g, float b, float a = 1f)
        {
            R = Math.Clamp(r, 0f, 1f);
            G = Math.Clamp(g, 0f, 1f);
            B = Math.Clamp(b, 0f, 1f);
            A = Math.Clamp(a, 0f, 1f);
        }

        public static ColorValue White => new(1, 1, 1, 1);
        public static ColorValue Black => new(0, 0, 0, 1);
        public static ColorValue Clear => new(1, 1, 1, 0);
        public static ColorValue Red => new(1, 0, 0, 1);
        public static ColorValue Green => new(0, 1, 0, 1);
        public static ColorValue Blue => new(0, 0, 1, 1);

        public bool Equals(ColorValue other) =>
            Math.Abs(R - other.R) < float.Epsilon &&
            Math.Abs(G - other.G) < float.Epsilon &&
            Math.Abs(B - other.B) < float.Epsilon &&
            Math.Abs(A - other.A) < float.Epsilon;

        public override bool Equals(object obj) => obj is ColorValue other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(R, G, B, A);

        public static bool operator ==(ColorValue left, ColorValue right) => left.Equals(right);
        public static bool operator !=(ColorValue left, ColorValue right) => !left.Equals(right);

        public override string ToString() => $"RGBA({R:F2}, {G:F2}, {B:F2}, {A:F2})";
    }
}
