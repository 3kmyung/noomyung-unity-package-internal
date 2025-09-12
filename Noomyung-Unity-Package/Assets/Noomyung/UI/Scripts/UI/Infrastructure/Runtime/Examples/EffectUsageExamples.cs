using System.Drawing;
using System.Numerics;
using UnityEngine;
using Noomyung.UI.Domain.ValueObjects;
using Noomyung.UI.Domain.Enums;

namespace Noomyung.UI.Infrastructure.Runtime.Examples
{
    /// <summary>
    /// 새로운 Effect 구조의 사용 예제입니다.
    /// </summary>
    public class EffectUsageExamples : MonoBehaviour
    {
        /// <summary>
        /// 타입 안전한 Effect 생성 예제
        /// </summary>
        public void CreateTypedEffects()
        {
            // 1. Fade Effect 생성
            var fadeData = new FadeEffectData(from: 0f, to: 1f);
            var fadeEffect = new Effect(
                type: EffectType.Fade,
                timing: new EffectTiming(duration: 1f, delay: 0f, repeat: 1),
                easing: new EffectEasing(EasingType.EaseInOut),
                data: fadeData
            );

            // 2. Scale Effect 생성
            var scaleData = new ScaleEffectData(
                from: Vector3.Zero,
                to: Vector3.One,
                axisMask: "XYZ"
            );
            var scaleEffect = new Effect(
                type: EffectType.Scale,
                timing: new EffectTiming(duration: 0.5f, delay: 0.2f, repeat: 1),
                easing: new EffectEasing(EasingType.EaseOut),
                data: scaleData
            );

            // 3. Move Effect 생성
            var moveData = new MoveEffectData(
                from: Vector3.Zero,
                to: new Vector3(100, 50, 0),
                space: "Anchored"
            );
            var moveEffect = new Effect(
                type: EffectType.Move,
                timing: new EffectTiming(duration: 2f, delay: 0f, repeat: 1),
                easing: new EffectEasing(EasingType.EaseInOut),
                data: moveData
            );

            // 4. Color Effect 생성
            var colorData = new ColorEffectData(
                from: Color.White,
                to: Color.Red,
                targetMode: "Graphic"
            );
            var colorEffect = new Effect(
                type: EffectType.Color,
                timing: new EffectTiming(duration: 1f, delay: 0f, repeat: 1),
                easing: new EffectEasing(EasingType.Linear),
                data: colorData
            );

            // 5. Shake Effect 생성
            var shakeData = new ShakeEffectData(
                strength: new Vector3(10, 10, 0),
                frequency: 15f,
                vibrato: 10
            );
            var shakeEffect = new Effect(
                type: EffectType.Shake,
                timing: new EffectTiming(duration: 0.5f, delay: 0f, repeat: 1),
                easing: new EffectEasing(EasingType.Linear),
                data: shakeData
            );
        }

        /// <summary>
        /// 타입 안전한 Effect 데이터 접근 예제
        /// </summary>
        public void AccessEffectDataSafely(Effect effect)
        {
            switch (effect.Type)
            {
                case EffectType.Fade:
                    if (effect.TryGetData<FadeEffectData>(out var fadeData))
                    {
                        Debug.Log($"Fade from {fadeData.From} to {fadeData.To}");
                    }
                    break;

                case EffectType.Scale:
                    if (effect.TryGetData<ScaleEffectData>(out var scaleData))
                    {
                        Debug.Log($"Scale from {scaleData.From} to {scaleData.To} with mask {scaleData.AxisMask}");
                    }
                    break;

                case EffectType.Move:
                    if (effect.TryGetData<MoveEffectData>(out var moveData))
                    {
                        Debug.Log($"Move from {moveData.From} to {moveData.To} in {moveData.Space} space");
                    }
                    break;

                case EffectType.Color:
                    if (effect.TryGetData<ColorEffectData>(out var colorData))
                    {
                        Debug.Log($"Color from {colorData.From} to {colorData.To} for {colorData.TargetMode}");
                    }
                    break;

                case EffectType.Shake:
                    if (effect.TryGetData<ShakeEffectData>(out var shakeData))
                    {
                        Debug.Log($"Shake with strength {shakeData.Strength}, frequency {shakeData.Frequency}");
                    }
                    break;

                default:
                    Debug.LogWarning($"Unsupported effect type: {effect.Type}");
                    break;
            }
        }

        /// <summary>
        /// ScriptableObject에서 새로운 Effect 생성 예제
        /// </summary>
        public void CreateEffectFromScriptableObject()
        {
            // ScriptableObject에서 EffectData 생성
            var moveAsset = ScriptableObject.CreateInstance<MoveEffectAsset>();
            var moveData = moveAsset.CreateEffectData();

            // Effect 생성
            var effect = new Effect(
                type: EffectType.Move,
                timing: new EffectTiming(duration: 1f, delay: 0f, repeat: 1),
                easing: new EffectEasing(EasingType.EaseInOut),
                data: moveData
            );

            Debug.Log($"Created effect from ScriptableObject: {effect}");
        }

        /// <summary>
        /// 타입 안전성 검증 예제
        /// </summary>
        public void DemonstrateTypeSafety()
        {
            // 올바른 타입으로 접근
            var fadeEffect = new Effect(
                EffectType.Fade,
                new EffectTiming(1f, 0f, 1),
                new EffectEasing(EasingType.Linear),
                new FadeEffectData(0f, 1f)
            );

            // 타입 안전한 접근
            var fadeData = fadeEffect.GetData<FadeEffectData>();
            Debug.Log($"Fade data: {fadeData.From} -> {fadeData.To}");

            // 잘못된 타입으로 접근 시도 (기본값 반환)
            var wrongData = fadeEffect.GetData<ScaleEffectData>();
            Debug.Log($"Wrong data (should be default): {wrongData.From} -> {wrongData.To}");

            // TryGetData로 안전한 접근
            if (fadeEffect.TryGetData<FadeEffectData>(out var safeFadeData))
            {
                Debug.Log($"Safe access successful: {safeFadeData.From} -> {safeFadeData.To}");
            }
            else
            {
                Debug.LogError("Failed to get FadeEffectData");
            }
        }
    }
}
