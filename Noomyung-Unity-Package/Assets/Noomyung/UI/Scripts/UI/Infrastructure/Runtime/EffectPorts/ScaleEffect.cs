using System.Threading;
using System.Numerics;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Noomyung.UI.Application.Ports;
using Noomyung.UI.Domain.Enums;

namespace Noomyung.UI.Infrastructure.Runtime.EffectPorts
{
    /// <summary>
    /// 스케일 효과를 처리하는 포트입니다.
    /// </summary>
    public class ScaleEffect : IEffectPort
    {
        private readonly bool _ignoreTimeScale;

        public ScaleEffect(bool ignoreTimeScale = true)
        {
            _ignoreTimeScale = ignoreTimeScale;
        }

        public async UniTask ExecuteAsync(IUIElementHandle target, Effect effect, bool reverse, CancellationToken cancellationToken = default)
        {
            var from = effect.GetVector3("From", Vector3.Zero);
            var to = effect.GetVector3("To", Vector3.One);
            var axisMask = (AxisMask)System.Enum.Parse(typeof(AxisMask), effect.GetString("AxisMask", "XYZ"));

            if (reverse) (from, to) = (to, from);

            var currentScale = target.LocalScale;

            await AnimateAsync(effect.Timing.Duration, cancellationToken, progress =>
            {
                var easedProgress = ApplyEasing(progress, effect.Easing);
                var lerpedScale = LerpVector3(from, to, easedProgress);

                // 축 마스크 적용
                var finalScale = currentScale;
                if (axisMask.HasFlag(AxisMask.X)) finalScale = new Vector3(lerpedScale.X, finalScale.Y, finalScale.Z);
                if (axisMask.HasFlag(AxisMask.Y)) finalScale = new Vector3(finalScale.X, lerpedScale.Y, finalScale.Z);
                if (axisMask.HasFlag(AxisMask.Z)) finalScale = new Vector3(finalScale.X, finalScale.Y, lerpedScale.Z);

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

        private Vector3 LerpVector3(Vector3 a, Vector3 b, float t)
        {
            return new Vector3(
                Mathf.Lerp(a.X, b.X, t),
                Mathf.Lerp(a.Y, b.Y, t),
                Mathf.Lerp(a.Z, b.Z, t));
        }
    }
}
