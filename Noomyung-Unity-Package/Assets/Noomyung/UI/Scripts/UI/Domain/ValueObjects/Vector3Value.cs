using System;

namespace Noomyung.UI.Domain.ValueObjects
{
    /// <summary>
    /// Unity에 종속되지 않는 3D 벡터 값 객체입니다.
    /// </summary>
    public readonly struct Vector3Value : IEquatable<Vector3Value>
    {
        public float X { get; }
        public float Y { get; }
        public float Z { get; }

        public Vector3Value(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public static Vector3Value Zero => new(0, 0, 0);
        public static Vector3Value One => new(1, 1, 1);

        public bool Equals(Vector3Value other) =>
            Math.Abs(X - other.X) < float.Epsilon &&
            Math.Abs(Y - other.Y) < float.Epsilon &&
            Math.Abs(Z - other.Z) < float.Epsilon;

        public override bool Equals(object obj) => obj is Vector3Value other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(X, Y, Z);

        public static bool operator ==(Vector3Value left, Vector3Value right) => left.Equals(right);
        public static bool operator !=(Vector3Value left, Vector3Value right) => !left.Equals(right);

        public override string ToString() => $"({X:F2}, {Y:F2}, {Z:F2})";
    }
}
