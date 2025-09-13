using System;
using System.Numerics;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Noomyung.UI.Application.Ports;
using Noomyung.UI.Domain.Enums;

namespace Noomyung.UI.Domain.ValueObjects.Effects
{
    /// <summary>
    /// Moves UI elements between positions with configurable space and timing.
    /// </summary>
    public class MoveEffect : IEffect
    {
        public EffectType Type => EffectType.Move;
        public EffectTiming Timing { get; }
        public EffectEasing Easing { get; }

        public Vector3 From { get; }
        public Vector3 To { get; }
        public string Space { get; }

        private readonly bool _ignoreTimeScale;

        public MoveEffect(
            Vector3 from,
            Vector3 to,
            EffectTiming timing,
            EffectEasing easing,
            string space = "Anchored",
            bool ignoreTimeScale = true)
        {
            From = from;
            To = to;
            Timing = timing ?? throw new ArgumentNullException(nameof(timing));
            Easing = easing ?? throw new ArgumentNullException(nameof(easing));
            Space = space;
            _ignoreTimeScale = ignoreTimeScale;
        }

        public async UniTask ExecuteAsync(IUIElementHandle target, bool reverse, CancellationToken cancellationToken = default)
        {
            var from = From;
            var to = To;

            if (reverse) (from, to) = (to, from);

            await AnimateAsync(Timing.Duration, cancellationToken, progress =>
            {
                var easedProgress = ApplyEasing(progress, Easing);
                var lerpedPosition = LerpVector3(from, to, easedProgress);

                if (Space == "Local")
                    target.LocalPosition = lerpedPosition;
                else
                    target.AnchoredPosition = lerpedPosition;
            });
        }

        private async UniTask AnimateAsync(float duration, CancellationToken cancellationToken, System.Action<float> onUpdate)
        {
            float elapsed = 0f;

            while (elapsed < duration && !cancellationToken.IsCancellationRequested)
            {
                var progress = Mathf.Clamp01(elapsed / duration);
                onUpdate?.Invoke(progress);

                await UniTask.Yield(cancellationToken: cancellationToken);
                elapsed += _ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;
            }

            if (!cancellationToken.IsCancellationRequested)
            {
                onUpdate?.Invoke(1f);
            }
        }

        private float ApplyEasing(float t, EffectEasing easing)
        {
            return easing.Type switch
            {
                EasingType.Linear => t,
                EasingType.EaseIn => t * t,
                EasingType.EaseOut => 1f - (1f - t) * (1f - t),
                EasingType.EaseInOut => t < 0.5f ? 2f * t * t : 1f - Mathf.Pow(-2f * t + 2f, 2f) / 2f,
                EasingType.CustomCurve when easing.OptionalCurveHandle is AnimationCurve curve => curve.Evaluate(t),
                _ => t
            };
        }

        private Vector3 LerpVector3(Vector3 a, Vector3 b, float t)
        {
            return new Vector3(
                Mathf.Lerp(a.X, b.X, t),
                Mathf.Lerp(a.Y, b.Y, t),
                Mathf.Lerp(a.Z, b.Z, t));
        }

        public override string ToString() => $"MoveEffect: {From} -> {To} in {Space} space";
    }
}
