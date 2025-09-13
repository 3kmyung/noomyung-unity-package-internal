namespace _3kmyung.Effect.Domain
{
    public record EffectEasing
    {
        public EasingType Type { get; }
        public AnimationCurve OptionalCurveHandle { get; }

        public EffectEasing(EasingType type, AnimationCurve optionalCurveHandle = null)
        {
            Type = type;
            OptionalCurveHandle = optionalCurveHandle;
        }
    }
}
