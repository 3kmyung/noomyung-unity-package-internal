using System.Collections.Generic;
using UnityEngine;
using Noomyung.UI.Domain.Enums;
using Noomyung.UI.Domain.ValueObjects;
using Noomyung.UI.Domain.ValueObjects.Effects;

namespace Noomyung.UI.Infrastructure.ScriptableObjects
{
    /// <summary>
    /// 페이드 효과 에셋입니다.
    /// </summary>
    public class FadeEffectAsset : UIEffectAsset
    {
        [Header("페이드 설정")]
        [SerializeField, Range(0f, 1f)] private float fromAlpha = 0f;
        [SerializeField, Range(0f, 1f)] private float toAlpha = 1f;

        /// <inheritdoc />
        public override EffectType EffectType => EffectType.Fade;

        /// <summary>시작 투명도</summary>
        public float FromAlpha => fromAlpha;

        /// <summary>끝 투명도</summary>
        public float ToAlpha => toAlpha;

        /// <inheritdoc />
        public override IReadOnlyDictionary<string, object> GetPayload()
        {
            return new Dictionary<string, object>
            {
                { "From", fromAlpha },
                { "To", toAlpha }
            };
        }

        /// <summary>
        /// 새로운 Effect 구조에 맞는 Effect를 생성합니다.
        /// </summary>
        public FadeEffect CreateEffect()
        {
            return new FadeEffect(
                from: fromAlpha,
                to: toAlpha,
                timing: new EffectTiming(duration: 1f, delay: 0f, repeat: 1),
                easing: new EffectEasing(EasingType.EaseInOut)
            );
        }
    }
}
