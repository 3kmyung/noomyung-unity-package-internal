using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using Noomyung.UI.Domain.Enums;
using Noomyung.UI.Domain.ValueObjects;
using Noomyung.UI.Domain.ValueObjects.Effects;

namespace Noomyung.UI.Infrastructure.ScriptableObjects
{
    /// <summary>
    /// 머티리얼 색상 속성 효과 에셋입니다.
    /// </summary>
    public class MaterialColorEffectAsset : UIEffectAsset
    {
        [Header("머티리얼 Color 설정")]
        [SerializeField] private string propertyName = "_Color";
        [SerializeField] private Color fromColor = Color.white;
        [SerializeField] private Color toColor = Color.white;

        /// <inheritdoc />
        public override EffectType EffectType => EffectType.MaterialColor;

        /// <summary>속성 이름</summary>
        public string PropertyName => propertyName;

        /// <summary>시작 색상</summary>
        public Color FromColor => fromColor;

        /// <summary>끝 색상</summary>
        public Color ToColor => toColor;

        /// <inheritdoc />
        public override IReadOnlyDictionary<string, object> GetPayload()
        {
            return new Dictionary<string, object>
            {
                { "PropertyName", propertyName },
                { "From", Color.FromArgb((int)(fromColor.a * 255), (int)(fromColor.r * 255), (int)(fromColor.g * 255), (int)(fromColor.b * 255)) },
                { "To", Color.FromArgb((int)(toColor.a * 255), (int)(toColor.r * 255), (int)(toColor.g * 255), (int)(toColor.b * 255)) }
            };
        }

        /// <summary>
        /// 새로운 Effect 구조에 맞는 Effect를 생성합니다.
        /// </summary>
        public MaterialColorEffect CreateEffect()
        {
            return new MaterialColorEffect(
                propertyName: propertyName,
                from: Color.FromArgb((int)(fromColor.a * 255), (int)(fromColor.r * 255), (int)(fromColor.g * 255), (int)(fromColor.b * 255)),
                to: Color.FromArgb((int)(toColor.a * 255), (int)(toColor.r * 255), (int)(toColor.g * 255), (int)(toColor.b * 255)),
                timing: new EffectTiming(duration: 1f, delay: 0f, repeat: 1),
                easing: new EffectEasing(EasingType.EaseInOut)
            );
        }

        /// <inheritdoc />
        protected override void OnValidate()
        {
            base.OnValidate();

            if (string.IsNullOrWhiteSpace(propertyName))
                propertyName = "_Color";
        }
    }
}
