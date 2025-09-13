using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Noomyung.UI.Application.Ports;
using Noomyung.UI.Domain.ValueObjects;

namespace Noomyung.UI.Infrastructure.Runtime.EffectPorts
{
    /// <summary>
    /// 머티리얼 컬러 효과를 처리하는 포트입니다.
    /// </summary>
    public class MaterialColorEffect : IEffectPort
    {
        private readonly bool _ignoreTimeScale;

        public MaterialColorEffect(bool ignoreTimeScale = true)
        {
            _ignoreTimeScale = ignoreTimeScale;
        }

        public async UniTask ExecuteAsync(IUIElementHandle target, Effect effect, bool reverse, CancellationToken cancellationToken = default)
        {
            var propertyName = effect.GetString("PropertyName", "_Color");
            var from = effect.GetColor("From", ColorValue.White);
            var to = effect.GetColor("To", ColorValue.White);

            if (reverse) (from, to) = (to, from);

            await AnimateAsync(effect.Timing.Duration, cancellationToken, progress =>
            {
                var easedProgress = ApplyEasing(progress, effect.Easing);
                var lerpedColor = LerpColor(from, to, easedProgress);
                target.SetMaterialColor(propertyName, lerpedColor);
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

        private ColorValue LerpColor(ColorValue a, ColorValue b, float t)
        {
            return new ColorValue(
                Mathf.Lerp(a.R, b.R, t),
                Mathf.Lerp(a.G, b.G, t),
                Mathf.Lerp(a.B, b.B, t),
                Mathf.Lerp(a.A, b.A, t));
        }
    }
}
