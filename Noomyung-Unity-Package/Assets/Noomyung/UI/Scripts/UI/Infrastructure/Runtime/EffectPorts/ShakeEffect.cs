using System.Threading;
using System.Numerics;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Noomyung.UI.Application.Ports;

namespace Noomyung.UI.Infrastructure.Runtime.EffectPorts
{
    /// <summary>
    /// 흔들기 효과를 처리하는 포트입니다.
    /// </summary>
    public class ShakeEffect : IEffectPort
    {
        private readonly bool _ignoreTimeScale;

        public ShakeEffect(bool ignoreTimeScale = true)
        {
            _ignoreTimeScale = ignoreTimeScale;
        }

        public async UniTask ExecuteAsync(IUIElementHandle target, Effect effect, bool reverse, CancellationToken cancellationToken = default)
        {
            if (!effect.TryGetData<ShakeEffectData>(out var shakeData))
            {
                Debug.LogError($"ShakeEffect: Invalid effect data type. Expected ShakeEffectData, got {effect.Data?.GetType().Name}");
                return;
            }

            var amplitude = shakeData.Strength.X; // X축을 amplitude로 사용
            var frequency = shakeData.Frequency;
            var duration = effect.Timing.Duration;

            target.StoreOriginalPosition();

            try
            {
                await AnimateAsync(duration, cancellationToken, progress =>
                {
                    var time = progress * duration;
                    var offsetX = Mathf.Sin(time * frequency * 2f * Mathf.PI) * amplitude * (1f - progress);
                    var offsetY = Mathf.Cos(time * frequency * 2f * Mathf.PI) * amplitude * (1f - progress);

                    var originalPos = target.AnchoredPosition;
                    target.AnchoredPosition = new Vector3(
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
