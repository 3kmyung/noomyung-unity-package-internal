using System.Collections.Generic;
using UnityEngine;
using Noomyung.UI.Domain.Enums;
using Noomyung.UI.Domain.ValueObjects;

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
                { "From", new ColorValue(fromColor.r, fromColor.g, fromColor.b, fromColor.a) },
                { "To", new ColorValue(toColor.r, toColor.g, toColor.b, toColor.a) }
            };
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
