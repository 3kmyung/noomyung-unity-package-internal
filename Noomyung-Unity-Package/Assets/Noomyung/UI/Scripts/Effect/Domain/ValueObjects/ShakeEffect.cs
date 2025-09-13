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
    /// 흔들림 효과를 나타내는 클래스입니다.
    /// </summary>
    public class ShakeEffect : IEffect
    {
        public EffectType Type => EffectType.Shake;
        public EffectTiming Timing { get; }
        public EffectEasing Easing { get; }

        /// <summary>흔들림 강도</summary>
        public Vector3 Strength { get; }

        /// <summary>흔들림 주파수</summary>
        public float Frequency { get; }

        /// <summary>진동 수</summary>
        public int Vibrato { get; }

        private readonly bool _ignoreTimeScale;

        public ShakeEffect(
            Vector3 strength,
            EffectTiming timing,
            EffectEasing easing,
            float frequency = 10f,
            int vibrato = 10,
            bool ignoreTimeScale = true)
        {
            Strength = strength;
            Timing = timing ?? throw new ArgumentNullException(nameof(timing));
            Easing = easing ?? throw new ArgumentNullException(nameof(easing));
            Frequency = frequency;
            Vibrato = vibrato;
            _ignoreTimeScale = ignoreTimeScale;
        }

        public async UniTask ExecuteAsync(IUIElementHandle target, bool reverse, CancellationToken cancellationToken = default)
        {
            var originalPosition = target.AnchoredPosition;
            var elapsed = 0f;

            while (elapsed < Timing.Duration && !cancellationToken.IsCancellationRequested)
            {
                var progress = Mathf.Clamp01(elapsed / Timing.Duration);
                var easedProgress = ApplyEasing(progress, Easing);

                // 흔들림 강도가 시간에 따라 감소
                var currentStrength = Strength * (1f - easedProgress);

                // 각 축별로 흔들림 계산
                var shakeOffset = new Vector3(
                    Mathf.Sin(elapsed * Frequency * Mathf.PI * 2f) * currentStrength.X,
                    Mathf.Sin(elapsed * Frequency * Mathf.PI * 2f + Mathf.PI / 2f) * currentStrength.Y,
                    Mathf.Sin(elapsed * Frequency * Mathf.PI * 2f + Mathf.PI) * currentStrength.Z
                );

                target.AnchoredPosition = originalPosition + shakeOffset;

                await UniTask.Yield(cancellationToken: cancellationToken);
                elapsed += _ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;
            }

            // 원래 위치로 복원
            if (!cancellationToken.IsCancellationRequested)
            {
                target.AnchoredPosition = originalPosition;
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

        public override string ToString() => $"ShakeEffect: Strength {Strength}, Frequency {Frequency}, Vibrato {Vibrato}";
    }
}
