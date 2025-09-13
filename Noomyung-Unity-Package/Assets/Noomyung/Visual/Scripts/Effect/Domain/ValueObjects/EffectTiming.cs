using System;

namespace _3kmyung.Effect.Domain
{
    /// <summary>
    /// Timing configuration for UI effects with cycle-based repetition semantics.
    /// </summary>
    public record EffectTiming
    {
        public float Duration { get; }

        public float Delay { get; }

        public PlaybackDirection Direction { get; }

        public RepeatMode RepeatMode { get; }

        public int CycleCount { get; }

        public EffectTiming(
            float duration = 1f,
            float delay = 0f,
            PlaybackDirection direction = PlaybackDirection.Forward,
            RepeatMode repeatMode = RepeatMode.Once,
            int cycleCount = 1)
        {
            Duration = duration >= 0f ? duration : throw new ArgumentException("Effect duration can't be lower than zero.", nameof(duration));
            Delay = delay >= 0f ? delay : throw new ArgumentException("Effect delay can't be lower than zero.", nameof(delay));
            Direction = direction;
            RepeatMode = repeatMode;
            CycleCount = cycleCount;
        }
    }
}
