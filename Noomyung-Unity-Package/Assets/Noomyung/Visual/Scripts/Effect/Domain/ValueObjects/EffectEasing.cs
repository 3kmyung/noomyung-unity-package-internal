using System;
using UnityEngine;
using _3kmyung.Effect.Domain.Enums;

namespace _3kmyung.Effect.Domain.ValueObjects
{
    public readonly struct EffectEasing : IEquatable<EffectEasing>
    {
        public EasingType Type { get; }
        public AnimationCurve OptionalCurveHandle { get; }

        public EffectEasing(EasingType type, AnimationCurve optionalCurveHandle = null)
        {
            Type = type;
            OptionalCurveHandle = optionalCurveHandle;
        }

        public bool Equals(EffectEasing other) =>
            Type == other.Type && Equals(OptionalCurveHandle, other.OptionalCurveHandle);

        public override bool Equals(object obj) => obj is EffectEasing other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(Type, OptionalCurveHandle);

        public static bool operator ==(EffectEasing left, EffectEasing right) => left.Equals(right);
        public static bool operator !=(EffectEasing left, EffectEasing right) => !left.Equals(right);

        public override string ToString() => $"Type: {Type}, Curve: {OptionalCurveHandle?.name ?? "null"}";
    }
}
