using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Noomyung.UI.Domain.Interfaces;
using Noomyung.UI.Domain.ValueObjects;
using Noomyung.UI.Domain.Enums;
using Noomyung.UI.Infrastructure.Async;

namespace Noomyung.UI.Infrastructure.Runtime.EffectPorts
{
    /// <summary>
    /// 스케일 효과를 처리하는 포트입니다.
    /// </summary>
    public class ScaleEffectPort : IEffectPort
    {
        private readonly IAsyncBridge _asyncBridge;
        private readonly bool _ignoreTimeScale;

        public ScaleEffectPort(IAsyncBridge asyncBridge = null, bool ignoreTimeScale = true)
        {
            _asyncBridge = asyncBridge ?? AsyncBridgeFactory.Create();
            _ignoreTimeScale = ignoreTimeScale;
        }

        public async UniTask ExecuteAsync(IUIElementHandle target, EffectStep step, bool reverse, CancellationToken cancellationToken = default)
        {
            var from = step.GetVector3("From", Vector3Value.Zero);
            var to = step.GetVector3("To", Vector3Value.One);
            var axisMask = (AxisMask)System.Enum.Parse(typeof(AxisMask), step.GetString("AxisMask", "XYZ"));

            if (reverse) (from, to) = (to, from);

            var currentScale = target.LocalScale;

            await AnimateAsync(step.Timing.Duration, cancellationToken, progress =>
            {
                var easedProgress = ApplyEasing(progress, step.Easing);
                var lerpedScale = LerpVector3(from, to, easedProgress);

                // 축 마스크 적용
                var finalScale = currentScale;
                if (axisMask.HasFlag(AxisMask.X)) finalScale = new Vector3Value(lerpedScale.X, finalScale.Y, finalScale.Z);
                if (axisMask.HasFlag(AxisMask.Y)) finalScale = new Vector3Value(finalScale.X, lerpedScale.Y, finalScale.Z);
                if (axisMask.HasFlag(AxisMask.Z)) finalScale = new Vector3Value(finalScale.X, finalScale.Y, lerpedScale.Z);

                target.LocalScale = finalScale;
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

        private Vector3Value LerpVector3(Vector3Value a, Vector3Value b, float t)
        {
            return new Vector3Value(
                Mathf.Lerp(a.X, b.X, t),
                Mathf.Lerp(a.Y, b.Y, t),
                Mathf.Lerp(a.Z, b.Z, t));
        }
    }
}
