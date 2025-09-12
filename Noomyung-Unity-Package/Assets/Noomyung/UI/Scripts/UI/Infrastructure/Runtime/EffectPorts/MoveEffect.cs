using System.Threading;
using System.Numerics;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Noomyung.UI.Application.Ports;

namespace Noomyung.UI.Infrastructure.Runtime.EffectPorts
{
    /// <summary>
    /// 이동 효과를 처리하는 포트입니다.
    /// </summary>
    public class MoveEffect : IEffectPort
    {
        private readonly bool _ignoreTimeScale;

        public MoveEffect(bool ignoreTimeScale = true)
        {
            _ignoreTimeScale = ignoreTimeScale;
        }

        public async UniTask ExecuteAsync(IUIElementHandle target, Effect effect, bool reverse, CancellationToken cancellationToken = default)
        {
            var from = effect.GetVector3("From", Vector3.Zero);
            var to = effect.GetVector3("To", Vector3.Zero);
            var space = effect.GetString("Space", "Anchored");

            if (reverse) (from, to) = (to, from);

            await AnimateAsync(effect.Timing.Duration, cancellationToken, progress =>
            {
                var easedProgress = ApplyEasing(progress, effect.Easing);
                var lerpedPosition = LerpVector3(from, to, easedProgress);

                if (space == "Local")
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
                Enums.EasingType.Linear => t,
                Enums.EasingType.EaseIn => t * t,
                Enums.EasingType.EaseOut => 1f - (1f - t) * (1f - t),
                Enums.EasingType.EaseInOut => t < 0.5f ? 2f * t * t : 1f - Mathf.Pow(-2f * t + 2f, 2f) / 2f,
                Enums.EasingType.CustomCurve when easing.OptionalCurveHandle is AnimationCurve curve => curve.Evaluate(t),
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
