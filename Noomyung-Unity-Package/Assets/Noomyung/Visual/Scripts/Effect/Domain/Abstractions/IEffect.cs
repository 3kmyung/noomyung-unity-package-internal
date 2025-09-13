namespace _3kmyung.Effect.Domain
{
    /// <summary>
    /// Represents a UI effect that can execute animations on UI elements.
    /// </summary>
    public interface IEffect
    {
        EffectType EffectType { get; }

        EffectTiming Timing { get; }

        EffectEasing Easing { get; }
    }
}
