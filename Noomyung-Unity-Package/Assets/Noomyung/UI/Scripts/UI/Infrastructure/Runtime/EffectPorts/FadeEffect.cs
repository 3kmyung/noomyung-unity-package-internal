using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Noomyung.UI.Application.Ports;
using Noomyung.UI.Domain.ValueObjects;

namespace Noomyung.UI.Infrastructure.Runtime.EffectPorts
{
    /// <summary>
    /// 페이드 효과를 처리하는 포트입니다.
    /// </summary>
    public class FadeEffect : IEffectPort
    {
        private readonly bool _ignoreTimeScale;

        public FadeEffect(bool ignoreTimeScale = true)
        {
            _ignoreTimeScale = ignoreTimeScale;
        }

        public async UniTask ExecuteAsync(IUIElementHandle target, Effect effect, bool reverse, CancellationToken cancellationToken = default)
        {
            if (!effect.TryGetData<FadeEffectData>(out var fadeData))
            {
                Debug.LogError($"FadeEffect: Invalid effect data type. Expected FadeEffectData, got {effect.Data?.GetType().Name}");
                return;
            }

            var from = fadeData.From;
            var to = fadeData.To;

            if (reverse) (from, to) = (to, from);

            await AnimateAsync(effect.Timing.Duration, cancellationToken, progress =>
            {
                var easedProgress = ApplyEasing(progress, effect.Easing);
                target.Alpha = Mathf.Lerp(from, to, easedProgress);
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
                Enums.EasingType.Linear => t,
                Enums.EasingType.EaseIn => t * t,
                Enums.EasingType.EaseOut => 1f - (1f - t) * (1f - t),
                Enums.EasingType.EaseInOut => t < 0.5f ? 2f * t * t : 1f - Mathf.Pow(-2f * t + 2f, 2f) / 2f,
                Enums.EasingType.CustomCurve when easing.OptionalCurveHandle is AnimationCurve curve => curve.Evaluate(t),
                _ => t
            };
        }
    }
}
