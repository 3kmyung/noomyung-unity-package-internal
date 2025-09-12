using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Noomyung.UI.Domain.Interfaces;
using Noomyung.UI.Domain.ValueObjects;
using Noomyung.UI.Infrastructure.Async;

namespace Noomyung.UI.Infrastructure.Runtime.EffectPorts
{
    /// <summary>
    /// 흔들기 효과를 처리하는 포트입니다.
    /// </summary>
    public class ShakeEffectPort : IEffectPort
    {
        private readonly IAsyncBridge _asyncBridge;
        private readonly bool _ignoreTimeScale;

        public ShakeEffectPort(IAsyncBridge asyncBridge = null, bool ignoreTimeScale = true)
        {
            _asyncBridge = asyncBridge ?? AsyncBridgeFactory.Create();
            _ignoreTimeScale = ignoreTimeScale;
        }

        public async UniTask ExecuteAsync(IUIElementHandle target, EffectStep step, bool reverse, CancellationToken cancellationToken = default)
        {
            var amplitude = step.GetFloat("Amplitude", 10f);
            var frequency = step.GetFloat("Frequency", 10f);
            var useDurationOverride = step.GetBool("UseDurationOverride", false);
            var durationOverride = step.GetFloat("DurationOverride", 0.5f);

            var duration = useDurationOverride ? durationOverride : step.Timing.Duration;

            target.StoreOriginalPosition();

            try
            {
                await AnimateAsync(duration, cancellationToken, progress =>
                {
                    var time = progress * duration;
                    var offsetX = Mathf.Sin(time * frequency * 2f * Mathf.PI) * amplitude * (1f - progress);
                    var offsetY = Mathf.Cos(time * frequency * 2f * Mathf.PI) * amplitude * (1f - progress);

                    var originalPos = target.AnchoredPosition;
                    target.AnchoredPosition = new Vector3Value(
                        originalPos.X + offsetX,
                        originalPos.Y + offsetY,
                        originalPos.Z);
                });
            }
            finally
            {
                target.RestoreOriginalPosition();
            }
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
    }
}
