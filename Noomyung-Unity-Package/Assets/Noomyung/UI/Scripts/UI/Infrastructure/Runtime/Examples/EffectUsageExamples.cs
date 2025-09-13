using System.Drawing;
using System.Numerics;
using UnityEngine;
using Noomyung.UI.Domain.ValueObjects;
using Noomyung.UI.Domain.ValueObjects.Effects;
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
            var fadeEffect = new FadeEffect(
                from: 0f,
                to: 1f,
                timing: new EffectTiming(duration: 1f, delay: 0f, repeat: 1),
                easing: new EffectEasing(EasingType.EaseInOut)
            );

            // 2. Scale Effect 생성
            var scaleEffect = new ScaleEffect(
                from: Vector3.Zero,
                to: Vector3.One,
                timing: new EffectTiming(duration: 0.5f, delay: 0.2f, repeat: 1),
                easing: new EffectEasing(EasingType.EaseOut),
                axisMask: "XYZ"
            );

            // 3. Move Effect 생성
            var moveEffect = new MoveEffect(
                from: Vector3.Zero,
                to: new Vector3(100, 50, 0),
                timing: new EffectTiming(duration: 2f, delay: 0f, repeat: 1),
                easing: new EffectEasing(EasingType.EaseInOut),
                space: "Anchored"
            );

            // 4. Color Effect 생성
            var colorEffect = new ColorEffect(
                from: Color.White,
                to: Color.Red,
                timing: new EffectTiming(duration: 1f, delay: 0f, repeat: 1),
                easing: new EffectEasing(EasingType.Linear),
                targetMode: "Graphic"
            );

            // 5. Shake Effect 생성
            var shakeEffect = new ShakeEffect(
                strength: new Vector3(10, 10, 0),
                timing: new EffectTiming(duration: 0.5f, delay: 0f, repeat: 1),
                easing: new EffectEasing(EasingType.Linear),
                frequency: 15f,
                vibrato: 10
            );
        }

        /// <summary>
        /// 타입 안전한 Effect 데이터 접근 예제
        /// </summary>
        public void AccessEffectDataSafely(IEffect effect)
        {
            switch (effect)
            {
                case FadeEffect fadeEffect:
                    Debug.Log($"Fade from {fadeEffect.From} to {fadeEffect.To}");
                    break;

                case ScaleEffect scaleEffect:
                    Debug.Log($"Scale from {scaleEffect.From} to {scaleEffect.To} with mask {scaleEffect.AxisMask}");
                    break;

                case MoveEffect moveEffect:
                    Debug.Log($"Move from {moveEffect.From} to {moveEffect.To} in {moveEffect.Space} space");
                    break;

                case ColorEffect colorEffect:
                    Debug.Log($"Color from {colorEffect.From} to {colorEffect.To} for {colorEffect.TargetMode}");
                    break;

                case ShakeEffect shakeEffect:
                    Debug.Log($"Shake with strength {shakeEffect.Strength}, frequency {shakeEffect.Frequency}");
                    break;

                default:
                    Debug.LogWarning($"Unsupported effect type: {effect.GetType().Name}");
                    break;
            }
        }

        /// <summary>
        /// ScriptableObject에서 새로운 Effect 생성 예제
        /// </summary>
        public void CreateEffectFromScriptableObject()
        {
            // ScriptableObject에서 Effect 생성
            var moveAsset = ScriptableObject.CreateInstance<MoveEffectAsset>();
            var moveEffect = moveAsset.CreateEffect();

            Debug.Log($"Created effect from ScriptableObject: {moveEffect}");
        }

        /// <summary>
        /// 타입 안전성 검증 예제
        /// </summary>
        public void DemonstrateTypeSafety()
        {
            // 올바른 타입으로 접근
            var fadeEffect = new FadeEffect(
                from: 0f,
                to: 1f,
                timing: new EffectTiming(1f, 0f, 1),
                easing: new EffectEasing(EasingType.Linear)
            );

            // 타입 안전한 접근 - 직접 속성에 접근
            Debug.Log($"Fade data: {fadeEffect.From} -> {fadeEffect.To}");

            // 인터페이스 매칭을 통한 안전한 접근
            if (fadeEffect is FadeEffect fade)
            {
                Debug.Log($"Safe access successful: {fade.From} -> {fade.To}");
            }
            else
            {
                Debug.LogError("Failed to cast to FadeEffect");
            }
        }
    }
}
