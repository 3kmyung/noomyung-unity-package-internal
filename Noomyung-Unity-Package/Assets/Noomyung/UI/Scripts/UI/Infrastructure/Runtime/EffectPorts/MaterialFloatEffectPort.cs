using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Noomyung.UI.Domain.Interfaces;
using Noomyung.UI.Domain.ValueObjects;
using Noomyung.UI.Infrastructure.Async;

namespace Noomyung.UI.Infrastructure.Runtime.EffectPorts
{
    /// <summary>
    /// 머티리얼 플로트 효과를 처리하는 포트입니다.
    /// </summary>
    public class MaterialFloatEffectPort : IEffectPort
    {
        private readonly IAsyncBridge _asyncBridge;
        private readonly bool _ignoreTimeScale;

        public MaterialFloatEffectPort(IAsyncBridge asyncBridge = null, bool ignoreTimeScale = true)
        {
            _asyncBridge = asyncBridge ?? AsyncBridgeFactory.Create();
            _ignoreTimeScale = ignoreTimeScale;
        }

        public async UniTask ExecuteAsync(IUIElementHandle target, EffectStep step, bool reverse, CancellationToken cancellationToken = default)
        {
            var propertyName = step.GetString("PropertyName", "_Alpha");
            var from = step.GetFloat("From", 0f);
            var to = step.GetFloat("To", 1f);

            if (reverse) (from, to) = (to, from);

            await AnimateAsync(step.Timing.Duration, cancellationToken, progress =>
            {
                var easedProgress = ApplyEasing(progress, step.Easing);
                var lerpedValue = Mathf.Lerp(from, to, easedProgress);
                target.SetMaterialFloat(propertyName, lerpedValue);
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
