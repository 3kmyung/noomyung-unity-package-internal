using System;
using _3kmyung.Effect.Domain.Enums;

namespace _3kmyung.Effect.Domain.ValueObjects
{
    /// <summary>
    /// Timing configuration for UI effects with cycle-based repetition semantics.
    /// </summary>
    public readonly struct EffectTiming : IEquatable<EffectTiming>
    {
        public float Duration { get; }
        public float Delay { get; }
        public PlaybackDirection Direction { get; }
        public RepeatMode RepeatMode { get; }
        public int CycleCount { get; }

        public EffectTiming(
            float duration,
            float delay = 0f,
            PlaybackDirection direction = PlaybackDirection.Forward,
            RepeatMode repeatMode = RepeatMode.Once,
            int cycleCount = 1)
        {
            Duration = Math.Max(0f, duration);
            Delay = Math.Max(0f, delay);
            Direction = direction;
            RepeatMode = repeatMode;

            if (repeatMode == RepeatMode.Finite)
            {
                CycleCount = Math.Max(1, cycleCount);
            }
            else
            {
                CycleCount = 1;
            }
        }

        public static EffectTiming Default => new(1f);

        public bool Equals(EffectTiming other) =>
            Math.Abs(Duration - other.Duration) < float.Epsilon &&
            Math.Abs(Delay - other.Delay) < float.Epsilon &&
            Direction == other.Direction &&
            RepeatMode == other.RepeatMode &&
            CycleCount == other.CycleCount;

        public override bool Equals(object obj) => obj is EffectTiming other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(Duration, Delay, Direction, RepeatMode, CycleCount);

        public static bool operator ==(EffectTiming left, EffectTiming right) => left.Equals(right);
        public static bool operator !=(EffectTiming left, EffectTiming right) => !left.Equals(right);

        public override string ToString() => $"Duration: {Duration}s, Delay: {Delay}s, Direction: {Direction}, Repeat: {RepeatMode}, Cycles: {CycleCount}";
    }
}
