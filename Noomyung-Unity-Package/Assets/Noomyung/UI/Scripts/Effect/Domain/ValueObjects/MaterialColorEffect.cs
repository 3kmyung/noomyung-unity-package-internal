using System;
using System.Drawing;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Noomyung.UI.Application.Ports;
using Noomyung.UI.Domain.Enums;

namespace Noomyung.UI.Domain.ValueObjects.Effects
{
    /// <summary>
    /// 머티리얼 Color 속성 효과를 나타내는 클래스입니다.
    /// </summary>
    public class MaterialColorEffect : IEffect
    {
        public EffectType Type => EffectType.MaterialColor;
        public EffectTiming Timing { get; }
        public EffectEasing Easing { get; }

        /// <summary>머티리얼 속성 이름</summary>
        public string PropertyName { get; }

        /// <summary>시작 색상</summary>
        public Color From { get; }

        /// <summary>목표 색상</summary>
        public Color To { get; }

        private readonly bool _ignoreTimeScale;

        public MaterialColorEffect(
            string propertyName,
            Color from,
            Color to,
            EffectTiming timing,
            EffectEasing easing,
            bool ignoreTimeScale = true)
        {
            PropertyName = propertyName ?? throw new ArgumentNullException(nameof(propertyName));
            From = from;
            To = to;
            Timing = timing ?? throw new ArgumentNullException(nameof(timing));
            Easing = easing ?? throw new ArgumentNullException(nameof(easing));
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
                var lerpedColor = LerpColor(from, to, easedProgress);
                target.SetMaterialColor(PropertyName, lerpedColor);
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

        private Color LerpColor(Color a, Color b, float t)
        {
            return Color.FromArgb(
                (int)Mathf.Lerp(a.A, b.A, t),
                (int)Mathf.Lerp(a.R, b.R, t),
                (int)Mathf.Lerp(a.G, b.G, t),
                (int)Mathf.Lerp(a.B, b.B, t)
            );
        }

        public override string ToString() => $"MaterialColorEffect: {PropertyName} {From} -> {To}";
    }
}
