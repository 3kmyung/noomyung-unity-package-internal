using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Noomyung.UI.Domain.Interfaces;
using Noomyung.UI.Domain.ValueObjects;
using Noomyung.UI.Domain.Enums;
using Noomyung.UI.Infrastructure.Async;

#if UNITASK_PRESENT
using Cysharp.Threading.Tasks;
#endif

namespace Noomyung.UI.Infrastructure.Runtime
{
    /// <summary>
    /// Unity 기반 효과 단계 실행기입니다.
    /// </summary>
    public class UnityEffectStepExecutor : IUIEffectStepExecutor
    {
        private readonly IAsyncBridge _asyncBridge;
        private readonly bool _ignoreTimeScale;

        /// <summary>
        /// UnityEffectStepExecutor의 새 인스턴스를 초기화합니다.
        /// </summary>
        /// <param name="asyncBridge">비동기 브리지</param>
        /// <param name="ignoreTimeScale">시간 스케일 무시 여부</param>
        public UnityEffectStepExecutor(IAsyncBridge asyncBridge = null, bool ignoreTimeScale = true)
        {
            _asyncBridge = asyncBridge ?? AsyncBridgeFactory.Create();
            _ignoreTimeScale = ignoreTimeScale;
        }

        /// <inheritdoc />
        public async Task ExecuteAsync(IUIElementHandle target, EffectStep step, CancellationToken cancellationToken = default)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            // 지연 시간 처리
            if (step.Timing.Delay > 0f)
            {
                await DelayAsync(step.Timing.Delay, cancellationToken);
            }

            // 기본 실행
            await ExecuteEffectAsync(target, step, cancellationToken);

            // 반복 처리
            for (int i = 0; i < step.Timing.Loops && !cancellationToken.IsCancellationRequested; i++)
            {
                if (step.Timing.LoopType == LoopType.Yoyo && i % 2 == 1)
                {
                    // Yoyo 효과를 위해 역방향 실행
                    await ExecuteReverseEffectAsync(target, step, cancellationToken);
                }
                else
                {
                    await ExecuteEffectAsync(target, step, cancellationToken);
                }
            }
        }

        private async Task ExecuteEffectAsync(IUIElementHandle target, EffectStep step, CancellationToken cancellationToken)
        {
            switch (step.Type)
            {
                case EffectType.Fade:
                    await ExecuteFadeAsync(target, step, false, cancellationToken);
                    break;
                case EffectType.Scale:
                    await ExecuteScaleAsync(target, step, false, cancellationToken);
                    break;
                case EffectType.Move:
                    await ExecuteMoveAsync(target, step, false, cancellationToken);
                    break;
                case EffectType.Color:
                    await ExecuteColorAsync(target, step, false, cancellationToken);
                    break;
                case EffectType.MaterialFloat:
                    await ExecuteMaterialFloatAsync(target, step, false, cancellationToken);
                    break;
                case EffectType.MaterialColor:
                    await ExecuteMaterialColorAsync(target, step, false, cancellationToken);
                    break;
                case EffectType.Shake:
                    await ExecuteShakeAsync(target, step, cancellationToken);
                    break;
            }
        }

        private async Task ExecuteReverseEffectAsync(IUIElementHandle target, EffectStep step, CancellationToken cancellationToken)
        {
            switch (step.Type)
            {
                case EffectType.Fade:
                    await ExecuteFadeAsync(target, step, true, cancellationToken);
                    break;
                case EffectType.Scale:
                    await ExecuteScaleAsync(target, step, true, cancellationToken);
                    break;
                case EffectType.Move:
                    await ExecuteMoveAsync(target, step, true, cancellationToken);
                    break;
                case EffectType.Color:
                    await ExecuteColorAsync(target, step, true, cancellationToken);
                    break;
                case EffectType.MaterialFloat:
                    await ExecuteMaterialFloatAsync(target, step, true, cancellationToken);
                    break;
                case EffectType.MaterialColor:
                    await ExecuteMaterialColorAsync(target, step, true, cancellationToken);
                    break;
                case EffectType.Shake:
                    await ExecuteShakeAsync(target, step, cancellationToken);
                    break;
            }
        }

        private async Task ExecuteFadeAsync(IUIElementHandle target, EffectStep step, bool reverse, CancellationToken cancellationToken)
        {
            var from = step.GetFloat("From", 0f);
            var to = step.GetFloat("To", 1f);
            
            if (reverse) (from, to) = (to, from);

            await AnimateAsync(step.Timing.Duration, cancellationToken, progress =>
            {
                var easedProgress = ApplyEasing(progress, step.Easing);
                target.Alpha = Mathf.Lerp(from, to, easedProgress);
            });
        }

        private async Task ExecuteScaleAsync(IUIElementHandle target, EffectStep step, bool reverse, CancellationToken cancellationToken)
        {
            var from = step.GetVector3("From", Vector3Value.Zero);
            var to = step.GetVector3("To", Vector3Value.One);
            var axisMask = (AxisMask)Enum.Parse(typeof(AxisMask), step.GetString("AxisMask", "XYZ"));
            
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

        private async Task ExecuteMoveAsync(IUIElementHandle target, EffectStep step, bool reverse, CancellationToken cancellationToken)
        {
            var from = step.GetVector3("From", Vector3Value.Zero);
            var to = step.GetVector3("To", Vector3Value.Zero);
            var space = step.GetString("Space", "Anchored");
            
            if (reverse) (from, to) = (to, from);

            await AnimateAsync(step.Timing.Duration, cancellationToken, progress =>
            {
                var easedProgress = ApplyEasing(progress, step.Easing);
                var lerpedPosition = LerpVector3(from, to, easedProgress);
                
                if (space == "Local")
                    target.LocalPosition = lerpedPosition;
                else
                    target.AnchoredPosition = lerpedPosition;
            });
        }

        private async Task ExecuteColorAsync(IUIElementHandle target, EffectStep step, bool reverse, CancellationToken cancellationToken)
        {
            var from = step.GetColor("From", ColorValue.White);
            var to = step.GetColor("To", ColorValue.White);
            var targetMode = step.GetString("TargetMode", "Graphic");
            
            if (reverse) (from, to) = (to, from);

            await AnimateAsync(step.Timing.Duration, cancellationToken, progress =>
            {
                var easedProgress = ApplyEasing(progress, step.Easing);
                var lerpedColor = LerpColor(from, to, easedProgress);
                
                if (targetMode == "AllGraphicsInChildren")
                    target.SetAllGraphicColors(lerpedColor);
                else
                    target.SetGraphicColor(0, lerpedColor);
            });
        }

        private async Task ExecuteMaterialFloatAsync(IUIElementHandle target, EffectStep step, bool reverse, CancellationToken cancellationToken)
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

        private async Task ExecuteMaterialColorAsync(IUIElementHandle target, EffectStep step, bool reverse, CancellationToken cancellationToken)
        {
            var propertyName = step.GetString("PropertyName", "_Color");
            var from = step.GetColor("From", ColorValue.White);
            var to = step.GetColor("To", ColorValue.White);
            
            if (reverse) (from, to) = (to, from);

            await AnimateAsync(step.Timing.Duration, cancellationToken, progress =>
            {
                var easedProgress = ApplyEasing(progress, step.Easing);
                var lerpedColor = LerpColor(from, to, easedProgress);
                target.SetMaterialColor(propertyName, lerpedColor);
            });
        }

        private async Task ExecuteShakeAsync(IUIElementHandle target, EffectStep step, CancellationToken cancellationToken)
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

        private async Task AnimateAsync(float duration, CancellationToken cancellationToken, Action<float> onUpdate)
        {
#if UNITASK_PRESENT
            var uniTask = AnimateUniTask(duration, cancellationToken, onUpdate);
            await _asyncBridge.ToTask(uniTask);
#else
            await AnimateStandardTask(duration, cancellationToken, onUpdate);
#endif
        }

#if UNITASK_PRESENT
        private async UniTask AnimateUniTask(float duration, CancellationToken cancellationToken, Action<float> onUpdate)
        {
            float elapsed = 0f;
            
            while (elapsed < duration && !cancellationToken.IsCancellationRequested)
            {
                var progress = Mathf.Clamp01(elapsed / duration);
                onUpdate?.Invoke(progress);
                
                await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
                elapsed += _ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;
            }
            
            if (!cancellationToken.IsCancellationRequested)
            {
                onUpdate?.Invoke(1f);
            }
        }
#endif

        private async Task AnimateStandardTask(float duration, CancellationToken cancellationToken, Action<float> onUpdate)
        {
            float elapsed = 0f;
            
            while (elapsed < duration && !cancellationToken.IsCancellationRequested)
            {
                var progress = Mathf.Clamp01(elapsed / duration);
                onUpdate?.Invoke(progress);
                
                await Task.Yield();
                elapsed += _ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;
            }
            
            if (!cancellationToken.IsCancellationRequested)
            {
                onUpdate?.Invoke(1f);
            }
        }

        private async Task DelayAsync(float seconds, CancellationToken cancellationToken)
        {
#if UNITASK_PRESENT
            var uniTask = UniTask.Delay(TimeSpan.FromSeconds(seconds), _ignoreTimeScale, cancellationToken: cancellationToken);
            await _asyncBridge.ToTask(uniTask);
#else
            await Task.Delay(TimeSpan.FromSeconds(seconds), cancellationToken);
#endif
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
